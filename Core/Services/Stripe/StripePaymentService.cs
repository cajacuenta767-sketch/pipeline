using Core.DTO.Stripe;
using Core.DTO.Stripe.Core.DTO.Stripe;
using Core.Entitys;
using Core.Enums;
using Core.Interfaces.Negocio;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.Stripe;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;


namespace Core.Services.Stripe
{
	public class StripePaymentService : IStripePaymentService
	{
		private readonly IUnitOfWorkSolicitudYonkes _unitOfWork;
		private readonly IUnitOfWorkNegocio _unitOfWorkNegocio;
		private readonly StripeSettings _stripeSettings;



		public StripePaymentService(IUnitOfWorkSolicitudYonkes unitOfWork,
									IUnitOfWorkNegocio unitOfWorkNegocio,
									IOptions<StripeSettings> stripeSettings)
		{
			_unitOfWork = unitOfWork;
			_unitOfWorkNegocio = unitOfWorkNegocio;
			_stripeSettings = stripeSettings.Value;

			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
		}

		public async Task<StripeCheckoutResponse> CrearCheckoutAsync(Guid ordenGuidId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			// ==========================================
			// 1. VALIDAR USUARIO
			// ==========================================

			if (usuarioId == Guid.Empty)
			{
				throw new UnauthorizedAccessException(
					"No se pudo identificar al usuario.");
			}


			// ==========================================
			// 2. OBTENER ORDEN
			// ==========================================

			var orden =
				await _unitOfWork
					.OrdenRepository
					.ObtenerPorGuidAsync(
						ordenGuidId,
						cancellationToken);

			if (orden == null)
			{
				throw new KeyNotFoundException(
					"La orden no existe.");
			}


			// ==========================================
			// 3. VALIDAR PROPIETARIO
			// ==========================================

			if (orden.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para pagar esta orden.");
			}


			// ==========================================
			// 4. VALIDAR IMPORTE
			// ==========================================

			if (orden.TotalCliente <= 0)
			{
				throw new InvalidOperationException(
					"La orden no tiene un importe válido.");
			}


			// ==========================================
			// 5. VALIDAR ESTADO DE ORDEN
			// ==========================================

			// Ajusta los IDs según tus EstatusOrden
			if (orden.EstatusOrdenId == 3)
			{
				throw new InvalidOperationException(
					"Esta orden ya fue pagada.");
			}


			// ==========================================
			// 6. OBTENER PAGO EXISTENTE
			// ==========================================

			var pagoExistente =
				await _unitOfWork
					.pagosRepository
					.ObtenerPorOrdenGuidAsync(
						orden.GuidId,
						cancellationToken);


			if (pagoExistente != null &&
				pagoExistente.Estado ==
					EstadoPagoCotizacion.Pagado)
			{
				throw new InvalidOperationException(
					"Esta orden ya fue pagada.");
			}


			// ==========================================
			// 7. OBTENER YONKE
			// ==========================================

			var yonke =
				await _unitOfWorkNegocio
					.YunkeRepository
					.getGuidById(
						orden.YonkeGuidId);


			if (yonke == null)
			{
				throw new KeyNotFoundException(
					"El yonke no existe.");
			}


			// ==========================================
			// 8. VALIDAR STRIPE CONNECT
			// ==========================================

			if (!yonke.StripeAccountConectada)
			{
				throw new InvalidOperationException(
					"El yonke todavía no tiene Stripe Connect habilitado.");
			}

			if (string.IsNullOrWhiteSpace(yonke.StripeAccountId))
			{
				throw new InvalidOperationException(
					"El yonke no tiene una cuenta Stripe Connect.");
			}

			// Limpiar espacios, saltos de línea, \r\n, etc.
			var stripeAccountId = yonke.StripeAccountId.Trim();

			if (string.IsNullOrWhiteSpace(stripeAccountId))
			{
				throw new InvalidOperationException(
					"El ID de la cuenta Stripe Connect no es válido.");
			}

			if (!stripeAccountId.StartsWith("acct_"))
			{
				throw new InvalidOperationException(
					$"El ID de Stripe Connect no tiene un formato válido: '{stripeAccountId}'.");
			}

			// ==========================================
			// 9. IMPORTES
			// ==========================================

			var totalCliente = orden.TotalCliente;
			var comisionRefanet = orden.ComisionRefanetImporte;
			var importeYonke = 	orden.ImporteYonke;


			if (comisionRefanet <= 0)
			{
				throw new InvalidOperationException(
					"La comisión de refaNet no es válida.");
			}


			if (importeYonke <= 0)
			{
				throw new InvalidOperationException(
					"El importe del yonke no es válido.");
			}


			// ==========================================
			// VALIDACIÓN DE SUMA
			// ==========================================

			var suma = 	importeYonke + 	comisionRefanet;


			if (suma != totalCliente)
			{
				throw new InvalidOperationException(
					"Los importes de la orden no coinciden.");
			}


			// ==========================================
			// 10. CENTAVOS
			// ==========================================

			var totalCentavos =
				checked(
					(long)Math.Round(
						totalCliente * 100m,
						0,
						MidpointRounding.AwayFromZero));


			var comisionCentavos =
				checked(
					(long)Math.Round(
						comisionRefanet * 100m,
						0,
						MidpointRounding.AwayFromZero));


			//Porcentaje de comision Refanet
			var porcentajeComisionPorcentaje = _stripeSettings.ComisionRefanetPorcentaje;

			var ComisionPorcentaje =
				Math.Round(
					totalCliente * porcentajeComisionPorcentaje / 100m,
					2,
					MidpointRounding.AwayFromZero);

		
			// ==========================================
			// 11. CREAR / RECUPERAR PAGO
			// ==========================================

			Pagos pago;

			if (pagoExistente == null)
			{
				pago = new Pagos
				{
					GuidId = Guid.NewGuid(),
					OrdenGuidId = orden.GuidId,
					CotizacionGuidId = orden.CotizacionGuidId,
					UsuarioId = usuarioId,
					Importe = totalCliente,
					ComisionRefanetPorcentaje = ComisionPorcentaje,
					GananciaRefanet = comisionRefanet,
					ImporteStripe = totalCliente,
					StripeAccountId = stripeAccountId,
					ImporteYonke = importeYonke,
					Moneda = "mxn",
					Estado = EstadoPagoCotizacion.Procesando,
					StripePaymentStatus = "unpaid",
					FechaCreacion = DateTime.UtcNow
				};

				await _unitOfWork.pagosRepository.AgregarAsync(
					pago,
					cancellationToken);
			}
			else
			{
				pago = pagoExistente;
				pago.Estado = EstadoPagoCotizacion.Procesando;
				pago.Error = null;
				pago.Importe = totalCliente;
				pago.ImporteYonke = importeYonke;
				pago.GananciaRefanet = comisionRefanet;
				pago.ImporteStripe = totalCliente;
				pago.StripeAccountId = stripeAccountId;
			}


			// ==========================================
			// 12. CREAR STRIPE CHECKOUT
			// ==========================================
			Console.WriteLine("****************************************");
			Console.WriteLine(">>> ENTRE AL METODO NUEVO DE CHECKOUT");
			Console.WriteLine("****************************************");

			var options = new SessionCreateOptions
				{
					Mode = "payment",

					SuccessUrl =
						$"{_stripeSettings.SuccessUrl}" +
						"?session_id={CHECKOUT_SESSION_ID}",

					CancelUrl =
						_stripeSettings.CancelUrl,


					LineItems =
						new List<SessionLineItemOptions>
						{
					new SessionLineItemOptions
					{
						Quantity = 1,

						PriceData =
							new SessionLineItemPriceDataOptions
							{
								Currency = "mxn",

								UnitAmount =
									totalCentavos,

								ProductData =
									new SessionLineItemPriceDataProductDataOptions
									{
										Name = "Compra RefaNet",

										Description = $"Orden {orden.GuidId}"
									}
							}
					}
				},


					// ======================================
					// METADATA CHECKOUT
					// ======================================

					Metadata =
						new Dictionary<string, string>
						{
							["OrdenGuidId"] = orden.GuidId.ToString(),
							 
							["PagoGuidId"] = pago.GuidId.ToString(),
							 
							["UsuarioId"] = usuarioId.ToString(),

							["YonkeGuidId"] = orden.YonkeGuidId.ToString()
						},


					// ======================================
					// PAYMENT INTENT
					// ======================================

					PaymentIntentData =
						new SessionPaymentIntentDataOptions
						{
							ApplicationFeeAmount =
								comisionCentavos,

							TransferData =
								new SessionPaymentIntentDataTransferDataOptions
								{
									Destination = stripeAccountId
								},


							Metadata =
								new Dictionary<string, string>
								{
									["OrdenGuidId"] =
										orden.GuidId.ToString(),

									["PagoGuidId"] =
										pago.GuidId.ToString(),

									["UsuarioId"] =
										usuarioId.ToString(),

									["YonkeGuidId"] =
										orden.YonkeGuidId.ToString()
								}
						}
				};


			// ==========================================
			// 13. CREAR SESSION
			// ==========================================

			Console.WriteLine("==========================================");
			Console.WriteLine(">>> CREANDO CHECKOUT");
			Console.WriteLine($">>> Currency: {options.LineItems[0].PriceData.Currency}");
			Console.WriteLine($">>> UnitAmount: {options.LineItems[0].PriceData.UnitAmount}");
			Console.WriteLine($">>> Metadata Count: {options.Metadata?.Count}");
			Console.WriteLine($">>> OrdenGuidId: {orden.GuidId}");
			Console.WriteLine($">>> PagoGuidId: {pago.GuidId}");
			Console.WriteLine($">>> StripeAccountId: {stripeAccountId}");
			Console.WriteLine("==========================================");

			var sessionService = new SessionService();


			var session =
				await sessionService.CreateAsync(
					options,
					cancellationToken: cancellationToken);

			Console.WriteLine("==========================================");
			Console.WriteLine(">>> CHECKOUT CREADO");
			Console.WriteLine($">>> Session ID: {session.Id}");
			Console.WriteLine($">>> URL: {session.Url}");
			Console.WriteLine("==========================================");


			if (session == null ||
				string.IsNullOrWhiteSpace(session.Id))
			{
				throw new InvalidOperationException(
					"Stripe no pudo crear la sesión.");
			}


			// ==========================================
			// 14. GUARDAR STRIPE
			// ==========================================

			pago.StripeCheckoutSessionId = session.Id;
			pago.StripePaymentIntentId = session.PaymentIntentId;
			pago.StripePaymentStatus = session.PaymentStatus;

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			// ==========================================
			// 15. RESPUESTA
			// ==========================================

			return new StripeCheckoutResponse
			{
				SessionId =
					session.Id,

				CheckoutUrl =
					session.Url
			};
		}


		public async Task<bool> ProcesarWebhookAsync(string json, string signature, CancellationToken cancellationToken = default)
		{
			// ==========================================
			// 1. VALIDAR JSON
			// ==========================================

			if (string.IsNullOrWhiteSpace(json))
			{
				throw new InvalidOperationException(
					"El webhook está vacío.");
			}


			// ==========================================
			// 2. VALIDAR FIRMA
			// ==========================================

			if (string.IsNullOrWhiteSpace(signature))
			{
				throw new InvalidOperationException(
					"Falta la firma de Stripe.");
			}


			// ==========================================
			// 3. CONSTRUIR EVENTO STRIPE
			// ==========================================

			Event stripeEvent;

			try
			{
				stripeEvent =
					EventUtility.ConstructEvent(
						json,
						signature,
						_stripeSettings.WebhookSecret,
						throwOnApiVersionMismatch: false);
			}
			catch (StripeException)
			{
				throw new InvalidOperationException(
					"Firma del webhook de Stripe inválida.");
			}


			// ==========================================
			// 4. OBTENER EVENT ID
			// ==========================================

			var eventId =
				stripeEvent.Id;

			if (string.IsNullOrWhiteSpace(eventId))
			{
				Console.WriteLine(
					">>> ERROR: Stripe EventId está vacío.");

				return false;
			}


			// ==========================================
			// 5. INFORMACIÓN GENERAL DEL EVENTO
			// ==========================================

			Console.WriteLine(
				"==========================================");

			Console.WriteLine(
				">>> STRIPE WEBHOOK RECIBIDO");

			Console.WriteLine(
				$">>> Event ID: {eventId}");

			Console.WriteLine(
				$">>> Event Type: {stripeEvent.Type}");

			Console.WriteLine(
				$">>> Live Mode: {stripeEvent.Livemode}");

			Console.WriteLine(
				"==========================================");


			// ==========================================
			// 6. PROCESAR EVENTO
			// ==========================================

			switch (stripeEvent.Type)
			{
				// ======================================
				// CHECKOUT COMPLETADO
				// ======================================

				case EventTypes.CheckoutSessionCompleted:

					Console.WriteLine(
						">>> ENTRÓ: checkout.session.completed");


					// ==================================
					// OBTENER SESSION
					// ==================================

					var session =
						stripeEvent.Data.Object as Session;


					if (session == null)
					{
						Console.WriteLine(
							">>> ERROR: Session es NULL.");

						return false;
					}


					// ==================================
					// MOSTRAR SESSION ID
					// ==================================

					Console.WriteLine(
						$">>> Session ID: {session.Id}");


					// ==================================
					// MOSTRAR PAYMENT INTENT
					// ==================================

					Console.WriteLine(
						$">>> PaymentIntent ID: {session.PaymentIntentId}");


					// ==================================
					// MOSTRAR STATUS DEL CHECKOUT
					// ==================================

					Console.WriteLine(
						$">>> Checkout Status: {session.Status}");

					Console.WriteLine(
						$">>> Payment Status: {session.PaymentStatus}");


					// ==================================
					// MOSTRAR AMOUNT TOTAL
					// ==================================

					Console.WriteLine(
						$">>> Amount Total: {session.AmountTotal}");

					Console.WriteLine(
						$">>> Currency: {session.Currency}");


					// ==================================
					// MOSTRAR METADATA
					// ==================================

					Console.WriteLine(
						">>> Metadata de Checkout:");

					if (session.Metadata != null &&
						session.Metadata.Count > 0)
					{
						foreach (var item in session.Metadata)
						{
							Console.WriteLine(
								$"    {item.Key} = {item.Value}");
						}
					}
					else
					{
						Console.WriteLine(
							">>> ADVERTENCIA: Checkout no tiene Metadata.");
					}


					// ==================================
					// EXTRAER DATOS IMPORTANTES
					// ==================================

					if (session.Metadata != null)
					{
						if (session.Metadata.TryGetValue(
							"OrdenGuidId",
							out var ordenGuidId))
						{
							Console.WriteLine(
								$">>> OrdenGuidId: {ordenGuidId}");
						}
						else
						{
							Console.WriteLine(
								">>> ADVERTENCIA: Falta OrdenGuidId.");
						}


						if (session.Metadata.TryGetValue(
							"PagoGuidId",
							out var pagoGuidId))
						{
							Console.WriteLine(
								$">>> PagoGuidId: {pagoGuidId}");
						}
						else
						{
							Console.WriteLine(
								">>> ADVERTENCIA: Falta PagoGuidId.");
						}


						if (session.Metadata.TryGetValue(
							"UsuarioId",
							out var usuarioId))
						{
							Console.WriteLine(
								$">>> UsuarioId: {usuarioId}");
						}
						else
						{
							Console.WriteLine(
								">>> ADVERTENCIA: Falta UsuarioId.");
						}


						if (session.Metadata.TryGetValue(
							"YonkeGuidId",
							out var yonkeGuidId))
						{
							Console.WriteLine(
								$">>> YonkeGuidId: {yonkeGuidId}");
						}
						else
						{
							Console.WriteLine(
								">>> ADVERTENCIA: Falta YonkeGuidId.");
						}
					}


					// ==================================
					// PROCESAR CHECKOUT
					// ==================================

					Console.WriteLine(
						">>> INICIANDO ProcesarCheckoutCompletadoAsync");


					var checkoutProcesado =
						await ProcesarCheckoutCompletadoAsync(
							session,
							eventId,
							cancellationToken);


					// ==================================
					// RESULTADO
					// ==================================

					Console.WriteLine(
						$">>> Resultado ProcesarCheckoutCompletadoAsync: {checkoutProcesado}");


					if (checkoutProcesado)
					{
						Console.WriteLine(
							">>> CHECKOUT PROCESADO CORRECTAMENTE");
					}
					else
					{
						Console.WriteLine(
							">>> ERROR: CHECKOUT NO PROCESADO");
					}


					Console.WriteLine(
						"==========================================");


					return checkoutProcesado;


				// ======================================
				// PAYMENT INTENT FALLIDO
				// ======================================

				case EventTypes.PaymentIntentPaymentFailed:

					Console.WriteLine(
						">>> ENTRÓ: payment_intent.payment_failed");


					var paymentIntent =
						stripeEvent.Data.Object as PaymentIntent;


					if (paymentIntent == null)
					{
						Console.WriteLine(
							">>> ERROR: PaymentIntent es NULL.");

						return false;
					}


					Console.WriteLine(
						$">>> PaymentIntent ID: {paymentIntent.Id}");


					Console.WriteLine(
						$">>> Amount: {paymentIntent.Amount}");

					Console.WriteLine(
						$">>> Currency: {paymentIntent.Currency}");

					Console.WriteLine(
						$">>> Status: {paymentIntent.Status}");


					// ==================================
					// METADATA PAYMENT INTENT
					// ==================================

					Console.WriteLine(
						">>> Metadata PaymentIntent:");

					if (paymentIntent.Metadata != null &&
						paymentIntent.Metadata.Count > 0)
					{
						foreach (var item in paymentIntent.Metadata)
						{
							Console.WriteLine(
								$"    {item.Key} = {item.Value}");
						}
					}
					else
					{
						Console.WriteLine(
							">>> PaymentIntent no tiene Metadata.");
					}


					// ==================================
					// PROCESAR PAGO FALLIDO
					// ==================================

					var pagoFallido =
						await ProcesarPagoFallidoAsync(
							paymentIntent,
							eventId,
							cancellationToken);


					Console.WriteLine(
						$">>> Resultado ProcesarPagoFallidoAsync: {pagoFallido}");


					return pagoFallido;


				// ======================================
				// REEMBOLSO
				// ======================================

				case EventTypes.ChargeRefunded:

					Console.WriteLine(
						">>> ENTRÓ: charge.refunded");


					var charge =
						stripeEvent.Data.Object as Charge;


					if (charge == null)
					{
						Console.WriteLine(
							">>> ERROR: Charge es NULL.");

						return false;
					}


					Console.WriteLine(
						$">>> Charge ID: {charge.Id}");


					Console.WriteLine(
						$">>> PaymentIntent ID: {charge.PaymentIntentId}");


					Console.WriteLine(
						$">>> Amount: {charge.Amount}");


					Console.WriteLine(
						$">>> Currency: {charge.Currency}");


					// ==================================
					// PROCESAR REEMBOLSO
					// ==================================

					var reembolsoProcesado =
						await ProcesarReembolsoAsync(
							charge,
							eventId,
							cancellationToken);


					Console.WriteLine(
						$">>> Resultado ProcesarReembolsoAsync: {reembolsoProcesado}");


					return reembolsoProcesado;


				// ======================================
				// EVENTOS NO MANEJADOS
				// ======================================

				default:

					Console.WriteLine(
						$">>> Evento no manejado: {stripeEvent.Type}");

					// Los eventos que no utilizamos
					// no deben provocar reintentos de Stripe.

					return true;
			}
		}



		private async Task<bool> ProcesarCheckoutCompletadoAsync(Session session, string stripeEventId, CancellationToken cancellationToken = default)
		{
			// ==========================================
			// 1. VALIDAR METADATA
			// ==========================================

			if (session.Metadata == null)
				return false;

			if (!session.Metadata.TryGetValue(
				"PagoGuidId",
				out var pagoGuidString))
			{
				return false;
			}

			if (!Guid.TryParse(
				pagoGuidString,
				out var pagoGuidId))
			{
				return false;
			}


			// ==========================================
			// 2. VALIDAR PAGO
			// ==========================================

			if (session.PaymentStatus != "paid")
				return false;


			// ==========================================
			// 3. OBTENER PAGO
			// ==========================================

			var pago =
				await _unitOfWork
					.pagosRepository
					.ObtenerPorGuidAsync(
						pagoGuidId,
						cancellationToken);

			if (pago == null)
				return false;


			// ==========================================
			// 4. IDEMPOTENCIA
			// ==========================================

			if (pago.Estado ==
				EstadoPagoCotizacion.Pagado)
			{
				return true;
			}


			// ==========================================
			// 5. DATOS DEL CHECKOUT
			// ==========================================

			pago.Estado =
				EstadoPagoCotizacion.Pagado;

			pago.FechaPago =
				DateTime.UtcNow;

			pago.StripeCheckoutSessionId =
				session.Id;

			pago.StripePaymentIntentId =
				session.PaymentIntentId;

			pago.StripePaymentStatus =
				session.PaymentStatus;

			pago.StripeEventId =
				stripeEventId;


			// ==========================================
			// 6. CUSTOMER
			// ==========================================

			if (!string.IsNullOrWhiteSpace(
				session.CustomerId))
			{
				pago.StripeCustomerId =
					session.CustomerId;
			}


			// ==========================================
			// 7. MONEDA
			// ==========================================

			if (!string.IsNullOrWhiteSpace(
				session.Currency))
			{
				pago.Moneda =
					session.Currency.ToUpper();
			}


			// ==========================================
			// 8. IMPORTE
			// ==========================================

			if (session.AmountTotal.HasValue)
			{
				pago.ImporteStripe =
					session.AmountTotal.Value / 100m;
			}


			// ==========================================
			// 9. OBTENER PAYMENT INTENT
			// ==========================================

			PaymentIntent? paymentIntent = null;

			if (!string.IsNullOrWhiteSpace(
				session.PaymentIntentId))
			{
				var paymentIntentService =
					new PaymentIntentService();

				paymentIntent =
					await paymentIntentService.GetAsync(
						session.PaymentIntentId,
						cancellationToken: cancellationToken);
			}


			// ==========================================
			// 10. DATOS DEL PAYMENT INTENT
			// ==========================================

			if (paymentIntent != null)
			{
				pago.StripePaymentIntentId =
					paymentIntent.Id;

				if (!string.IsNullOrWhiteSpace(
					paymentIntent.Status))
				{
					pago.StripePaymentStatus =
						paymentIntent.Status;
				}

				if (!string.IsNullOrWhiteSpace(
					paymentIntent.PaymentMethodId))
				{
					pago.StripePaymentMethodId =
						paymentIntent.PaymentMethodId;
				}


				// ==========================================
				// METADATA PAYMENT INTENT
				// ==========================================

				if (paymentIntent.Metadata != null)
				{
					if (paymentIntent.Metadata.TryGetValue(
						"StripeAccountId",
						out var stripeAccountId))
					{
						if (!string.IsNullOrWhiteSpace(
							stripeAccountId))
						{
							pago.StripeAccountId =
								stripeAccountId;
						}
					}
				}


				// ==========================================
				// CHARGE
				// ==========================================

				if (!string.IsNullOrWhiteSpace(
					paymentIntent.LatestChargeId))
				{
					pago.StripeChargeId =
						paymentIntent.LatestChargeId;
				}
			}


			// ==========================================
			// 11. OBTENER CHARGE
			// ==========================================

			Charge? charge = null;

			if (!string.IsNullOrWhiteSpace(
				pago.StripeChargeId))
			{
				var chargeService =
					new ChargeService();

				charge =
					await chargeService.GetAsync(
						pago.StripeChargeId,
						cancellationToken: cancellationToken);
			}


			// ==========================================
			// 12. DATOS DEL CHARGE
			// ==========================================

			if (charge != null)
			{
				pago.StripeChargeId =
					charge.Id;

				if (!string.IsNullOrWhiteSpace(
					charge.PaymentMethod))
				{
					pago.StripePaymentMethodId =
						charge.PaymentMethod;
				}


				// ==========================================
				// TRANSFER CONNECT
				// ==========================================

				if (!string.IsNullOrWhiteSpace(
					charge.TransferId))
				{
					pago.StripeTransferId =
						charge.TransferId;
				}


				// ==========================================
				// STRIPE FEE
				// ==========================================

				if (!string.IsNullOrWhiteSpace(
					charge.BalanceTransactionId))
				{
					var balanceTransactionService =
						new BalanceTransactionService();

					var balanceTransaction =
						await balanceTransactionService.GetAsync(
							charge.BalanceTransactionId,
							cancellationToken: cancellationToken);

					if (balanceTransaction != null)
					{
						pago.StripeFee =
							balanceTransaction.Fee / 100m;
					}
				}
			}


			// ==========================================
			// 13. CALCULAR IMPORTES REFANET
			// ==========================================

			// Importe = precio total que pagó el cliente

			if (pago.Importe <= 0 &&
				session.AmountTotal.HasValue)
			{
				pago.Importe =
					session.AmountTotal.Value / 100m;
			}


			// Comisión RefaNet

			if (pago.ComisionRefanetPorcentaje <= 0)
			{
				pago.ComisionRefanetPorcentaje =
					10m;
			}


			pago.ComisionRefanetImporte =
				Math.Round(
					pago.Importe *
					(pago.ComisionRefanetPorcentaje / 100m),
					2);


			// ==========================================
			// 14. IMPORTE YONKE
			// ==========================================

			pago.ImporteYonke =
				Math.Round(
					pago.Importe -
					pago.ComisionRefanetImporte,
					2);


			// ==========================================
			// 15. IMPORTE STRIPE
			// ==========================================

			if (pago.StripeFee.HasValue)
			{
				pago.ImporteStripe =
					Math.Round(
						pago.Importe -
						pago.StripeFee.Value,
						2);
			}
			else
			{
				pago.ImporteStripe =
					pago.Importe;
			}


			// ==========================================
			// 16. GANANCIA REFANET
			// ==========================================

			if (pago.StripeFee.HasValue)
			{
				pago.GananciaRefanet =
					Math.Round(
						pago.ComisionRefanetImporte -
						pago.StripeFee.Value,
						2);
			}
			else
			{
				pago.GananciaRefanet =
					pago.ComisionRefanetImporte;
			}


			// ==========================================
			// 17. OBTENER ORDEN
			// ==========================================

			var orden =
				await _unitOfWork
					.OrdenRepository
					.ObtenerPorGuidAsync(
						pago.OrdenGuidId,
						cancellationToken);

			if (orden == null)
				return false;


			// ==========================================
			// 18. ACTUALIZAR ORDEN
			// ==========================================

			orden.EstatusOrdenId =
				3;


			// ==========================================
			// 19. ACTUALIZAR PAGO
			// ==========================================

			await _unitOfWork
				.pagosRepository
				.ActualizarAsync(
					pago,
					cancellationToken);


			// ==========================================
			// 20. ACTUALIZAR ORDEN
			// ==========================================

			await _unitOfWork
				.OrdenRepository
				.ActualizarAsync(
					orden,
					cancellationToken);


			// ==========================================
			// 21. GUARDAR
			// ==========================================

			await _unitOfWork
				.SaveChangesAsync(
					cancellationToken);


			return true;


		}
		private async Task<bool> ProcesarPagoFallidoAsync(PaymentIntent paymentIntent, string stripeEventId, CancellationToken cancellationToken = default)
		{
			if (paymentIntent.Metadata == null)
				return false;


			if (!paymentIntent.Metadata.TryGetValue(
				"PagoGuidId",
				out var pagoGuidString))
			{
				return false;
			}


			if (!Guid.TryParse(
				pagoGuidString,
				out var pagoGuidId))
			{
				return false;
			}


			var pago =
				await _unitOfWork
					.pagosRepository
					.ObtenerPorGuidAsync(
						pagoGuidId,
						cancellationToken);


			if (pago == null)
				return false;


			pago.StripePaymentIntentId =
				paymentIntent.Id;

			pago.StripePaymentStatus =
				paymentIntent.Status;

			pago.Estado =
				EstadoPagoCotizacion.Fallido;

			pago.StripeEventId =
				stripeEventId;

			pago.Error =
				"El pago fue rechazado o falló en Stripe.";


			await _unitOfWork
				.pagosRepository
				.ActualizarAsync(
					pago,
					cancellationToken);


			await _unitOfWork
				.SaveChangesAsync(
					cancellationToken);


			return true;
		}
		private async Task<bool> ProcesarReembolsoAsync(Charge charge, string stripeEventId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(
				charge.PaymentIntentId))
			{
				return false;
			}


			var pago =
				await _unitOfWork
					.pagosRepository
					.ObtenerPorStripePaymentIntentAsync(
						charge.PaymentIntentId,
						cancellationToken);


			if (pago == null)
				return false;


			// ==========================================
			// IDEMPOTENCIA
			// ==========================================

			if (pago.Estado ==
				EstadoPagoCotizacion.Reembolsado)
			{
				return true;
			}


			// ==========================================
			// ACTUALIZAR PAGO
			// ==========================================

			pago.Estado =
				EstadoPagoCotizacion.Reembolsado;

			pago.StripePaymentStatus =
				"refunded";

			pago.FechaReembolso =
				DateTime.UtcNow;

			pago.StripeChargeId =
				charge.Id;

			pago.StripeEventId =
				stripeEventId;


			await _unitOfWork
				.pagosRepository
				.ActualizarAsync(
					pago,
					cancellationToken);


			// ==========================================
			// ACTUALIZAR ORDEN
			// ==========================================

			var orden =
				await _unitOfWork
					.OrdenRepository
					.ObtenerPorGuidAsync(
						pago.OrdenGuidId,
						cancellationToken);


			if (orden != null)
			{
				orden.EstatusOrdenId =
					10; // REEMBOLSADA


				await _unitOfWork
					.OrdenRepository
					.ActualizarAsync(
						orden,
						cancellationToken);
			}


			await _unitOfWork
				.SaveChangesAsync(
					cancellationToken);


			return true;
		}




		public async Task<PagoResultadoResponse> ObtenerResultadoPagoAsync(string sessionId, Guid usuarioId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(sessionId))
			{
				throw new ArgumentException(
					"El sessionId es obligatorio.");
			}

			if (usuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para pagar esta orden.");
			}

			// ==========================================
			// 1. BUSCAR PAGO POR SESSION ID
			// ==========================================

			var pago =
				await _unitOfWork
					.pagosRepository
					.ObtenerPorStripeCheckoutSessionAsync(
						sessionId,
						cancellationToken);

			if (pago == null)
			{
				throw new KeyNotFoundException(
					"No se encontró el pago asociado a la sesión de Stripe.");
			}

			// ==========================================
			// 2. VALIDAR PROPIETARIO
			// ==========================================

			if (pago.UsuarioId != usuarioId)
			{
				throw new UnauthorizedAccessException(
					"No tienes permiso para consultar este pago.");
			}

			// ==========================================
			// 3. DEVOLVER RESULTADO
			// ==========================================

			return new PagoResultadoResponse
			{
				Exitoso = pago.Estado == EstadoPagoCotizacion.Pagado,
				Estado = pago.Estado.ToString(),
				SessionId = pago.StripeCheckoutSessionId ?? sessionId,
				OrdenGuidId = pago.OrdenGuidId,
				PagoGuidId = pago.GuidId, 
				Importe = pago.Importe,
				Moneda = pago.Moneda, 
				Mensaje =
					pago.Estado == EstadoPagoCotizacion.Pagado
						? "El pago fue realizado correctamente."
						: "El pago todavía no ha sido confirmado."
			};
		}
	}
}
