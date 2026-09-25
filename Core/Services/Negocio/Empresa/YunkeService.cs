using AutoMapper;
using Core.DTO.Empresas;
using Core.Entitys;
using Core.Exceptions;
using Core.Interfaces.Auth.AccesoDetalle;
using Core.Interfaces.Azure;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Negocio;
using Core.Interfaces.Negocio.Empresa;
using Core.Interfaces.Utilerias;
using Core.Models.BuildSecurity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Services.Negocio.Empresa
{
	public class Yonkeservice : IYonkeservice
	{
		public readonly IUnitOfWorkNegocio _unitOfWorkNegocio;
		public readonly ICurrentUserService _currentUserService;
		public readonly IUnitOfWorkUtilerias _unitOfWorkUtilerias;
		public readonly IMapper _mapper;
		public readonly IAlmacenadorArchivos _almacenadorArchivos;

		private UserManager<IdentityUser> _userManager;

		//servicio de registro de usuarios
		private readonly IUserService _userService;

		//registro de acceso detalles de cada empresa
		private readonly IAccesoDetalleService _accesoDetalleService;

		
		public Yonkeservice(IUnitOfWorkNegocio unitOfWorkNegocio, 
							ICurrentUserService currentUserService,
							IUnitOfWorkUtilerias unitOfWorkUtilerias,
							IMapper mapper,
							IAlmacenadorArchivos almacenadorArchivos,
							IUserService userService,
							UserManager<IdentityUser> userManager,
							IAccesoDetalleService accesoDetalleService)
		{
			_unitOfWorkNegocio = unitOfWorkNegocio;			
			_currentUserService = currentUserService;
			_unitOfWorkUtilerias = unitOfWorkUtilerias;
			_mapper = mapper;
			_almacenadorArchivos = almacenadorArchivos;

			_userService = userService;
			_userManager = userManager;

			_accesoDetalleService = accesoDetalleService;
		}

		

		public async Task<IQueryable<YonkesListDTO>> getYonkesByEntidad(int? ciudadId)
		{
			return await _unitOfWorkNegocio.YunkeRepository.getYonkesByCiudad(ciudadId);
		}

		

		public async Task<YonkesaveDTO> NuevoYunkeAsync(YonkesaveDTO model,	CancellationToken cancellationToken = default)
		{
			await using var transaction = await _unitOfWorkNegocio.BeginTransactionAsync(cancellationToken);

			try
			{
				var usuarioId = _currentUserService.UserId;

				if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
				{
					throw new BusinessException(
						"No fue posible identificar al usuario.");
				}

				// Validar ciudad
				var ciudad = await _unitOfWorkUtilerias.CiudadesRepository.getCiudadById(model.CiudadId);

				if (ciudad == null)
				{
					throw new BusinessException("La ciudad no existe.");
				}


				// Validar duplicado
				var existe = await _unitOfWorkNegocio
					.YunkeRepository
					.ExisteAsync(
						model.Nombre,
						model.Correo,
						model.Telefono);

				if (existe)
				{
					throw new BusinessException("El yonke ya se encuentra registrado.");
				}


				// Mapear entidad
				var entidad = _mapper.Map<Yonkes>(model);

				entidad.GuidId = Guid.NewGuid();
				entidad.CreateBy = usuarioId.Value;
				entidad.CreateAt = DateTime.UtcNow;
				entidad.Autorizado = false;
				entidad.Estatus = true;


				// Guardar logotipo en Azure
				if (model.LogoUrl != null)
				{
					const string contenedor = "logos";

					var extension = Path.GetExtension(model.LogoUrl.FileName);

					var rutaBlob = $"empresas/{Guid.NewGuid()}{extension}";


					using var memoryStream = new MemoryStream();

					await model.LogoUrl.CopyToAsync(memoryStream, cancellationToken);


					entidad.LogoUrl =
						await _almacenadorArchivos.GuardarArchivo(
							memoryStream.ToArray(),
							extension,
							contenedor,
							rutaBlob,
							model.LogoUrl.ContentType);
				}


				// Guardar Yonke
				await _unitOfWorkNegocio.YunkeRepository.addYunke(entidad);


				var registros =	await _unitOfWorkNegocio.SaveChangesAsync(cancellationToken);


				if (registros <= 0)
				{
					throw new BusinessException("No fue posible guardar el yonke.");
				}


				// Crear usuario asociado
				if (string.IsNullOrWhiteSpace(model.Password))
				{
					throw new BusinessException("La contraseña es obligatoria.");
				}

				if (model.Password != model.ConfirmPassword)
				{
					throw new BusinessException("Las contraseñas no coinciden.");
				}

				var registroUsuario = new RegisterViewAspociadoModel
				{
					Email = entidad.Correo,
					Password = model.Password,
					ConfirmPassword = model.ConfirmPassword,
					PhoneNumber = entidad.Telefono,
					PhoneNumberConfirmed = false,
					EmpresaId = entidad.Id,
					Role = "Asociado"
				};


				var resultadoUsuario = await _userService.RegisterAsociadoAsync(registroUsuario);


				if (!resultadoUsuario.IsSuccess)
				{
					throw new BusinessException(
						resultadoUsuario.Message);
				}


				// Obtener usuario creado
				var usuario =
					await _userManager
						.FindByEmailAsync(entidad.Correo);


				if (usuario == null)
				{
					throw new BusinessException(
						"No fue posible obtener el usuario creado.");
				}

				// Crear acceso del usuario al yonke
				await _accesoDetalleService.InsertAccesoDetalle(new AccesoDetalles
				{
					UserId = usuario.Id,
					EmpresaId = entidad.Id,
					Estatus = true
				});


				await transaction.CommitAsync(cancellationToken);


				return _mapper.Map<YonkesaveDTO>(entidad);
			}
			catch
			{
				await transaction.RollbackAsync(CancellationToken.None);

				throw;
			}
		}

		public async Task<bool> UpdateYunke(Yonkes empresa)
		{
			// Obtener el yonke existente	
			var existdepen = await _unitOfWorkNegocio.YunkeRepository.getYunkeGuidById(empresa.GuidId);
			
			if (existdepen == null)
				throw new BusinessException("El yonke no existe.");

			var cityValida = await _unitOfWorkUtilerias
				.CiudadesRepository.getCiudadById(empresa.CiudadId);
			if (cityValida == null)
				throw new BusinessException("La ciudad no existe.");

			empresa.Id = existdepen.Id;
			empresa.CreateBy = existdepen?.CreateBy;
			empresa.LogoUrl = existdepen.LogoUrl;
			empresa.Correo = existdepen.Correo;

			_unitOfWorkNegocio.YunkeRepository.UpdateYunke(empresa);
			await _unitOfWorkNegocio.SaveChangesAsync();

			return true;
		}

		public async Task<bool> BajaYunke(Guid guidId)
		{
			//reglas de negocio a aplicar
			var existdepen = await _unitOfWorkNegocio.YunkeRepository.getGuidById(guidId);

			existdepen.Estatus = false;

			if (existdepen == null)
			{
				throw new BusinessException("Id de Yunke no Existe");
			}

			_unitOfWorkNegocio.YunkeRepository.BajaYunke(existdepen);
			await _unitOfWorkNegocio.SaveChangesAsync();

			return true;

		}

		

		public async Task<YonkesListDTO?> getYunkeByGuidId(Guid guidId)
		{
			return await _unitOfWorkNegocio.YunkeRepository.getYunkeGuidById(guidId);
		}

		public async Task<Yonkes?> getYunkeById(int Id)
		{
			return await _unitOfWorkNegocio.YunkeRepository.getYunkeById(Id);
		}

		public async Task<bool> ActualizarLogoAsync(Guid yonkeGuidId, IFormFile logo)
		{
			var usuarioId = _currentUserService.UserId;

			if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
			{
				throw new BusinessException(
					"No fue posible identificar al usuario.");
			}

			var yonke = await _unitOfWorkNegocio.YunkeRepository
				.getGuidById(yonkeGuidId);

			if (yonke == null)
				return false;

			const string contenedor = "logos";

			// Eliminar el logo anterior
			if (!string.IsNullOrWhiteSpace(yonke.LogoUrl))
			{
				await _almacenadorArchivos.BorrarArchivo(yonke.LogoUrl, contenedor);
			}

			var extension = Path.GetExtension(logo.FileName);
			var rutaBlob = $"empresas/{Guid.NewGuid()}{extension}";

			using var memoryStream = new MemoryStream();

			await logo.CopyToAsync(memoryStream);

			var logoUrl = await _almacenadorArchivos.GuardarArchivo(
				memoryStream.ToArray(),
				extension,
				contenedor,
				rutaBlob,
				logo.ContentType);

			yonke.LogoUrl = logoUrl;
			yonke.CreateBy = usuarioId.Value;
			yonke.CreateAt = yonke.CreateAt;

			_unitOfWorkNegocio.YunkeRepository.UpdateYunke(yonke);

			return await _unitOfWorkNegocio.SaveChangesAsync() > 0;
		}

		public async Task<Yonkes?> getYonkeGuidId(Guid guidId)
		{
			return await _unitOfWorkNegocio.YunkeRepository.getGuidById(guidId);
		}
	}
}
