using Core.Entitys;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataContext
{
	public class AplicationDBContext : IdentityDbContext
	{
		//envia las opciones de la conexion al StartUp.
		public AplicationDBContext(DbContextOptions<AplicationDBContext> options) : base(options)
		{
		}

		public virtual DbSet<Clientes> Clientes { get; set; }

		public virtual DbSet<ClienteOtps> ClienteOtps { get; set; }
		public virtual DbSet<UsuariosDispositivos> UsuariosDispositivos { get; set; }


		public virtual DbSet<AccesoDetalles> AccesoDetalles { get; set; }
		public virtual DbSet<Yonkes> Yonkes { get; set; }
		public virtual DbSet<YonkesCoberturas> YonkesCoberturas { get; set; }
		public virtual DbSet<Subscripciones> Subscripciones { get; set; }
		public virtual DbSet<SubcripcionPeriodos> SubcripcionPeriodos { get; set; }
		public virtual DbSet<YonkesDispositivos> YonkesDispositivos	{ get; set; }
		public virtual DbSet<YonkesCalificaciones> YonkesCalificaciones { get; set; }	


		public virtual DbSet<Entidades> Entidades { get; set; }
		public virtual DbSet<Ciudades> Ciudades { get; set; }


		public virtual DbSet<Marcas> Marcas { get; set; }
		public virtual DbSet<Modelos> Modelos { get; set; }



		public virtual DbSet<Solicitudes> Solicitudes { get; set; }
		public virtual DbSet<Folios> Folios { get; set; }	
		public virtual DbSet<SolicitudesCiudades> SolicitudesCiudades { get; set; }
		public virtual DbSet<SolicitudesHistorials> SolicitudesHistorials { get; set; }
		public virtual DbSet<SolicitudesEstatus> SolicitudesEstatus { get; set; }
		public virtual DbSet<SolicitudesImagenes> SolicitudesImagenes { get; set; }


		public virtual DbSet<SolicitudYonkes> SolicitudYonkes { get; set; }	
		public virtual DbSet<SolicitudYonkesEstatus> SolicitudYonkesEstatus { get; set; }


		public virtual DbSet<SolicitudCotizaciones> SolicitudCotizaciones { get; set; }
		public virtual DbSet<SolicitudCotizacionesImagenes> SolicitudCotizacionesImagenes { get; set; }
		public virtual DbSet<SolicitudCotizacionMensajes> SolicitudCotizacionMensajes { get; set; }


		public virtual DbSet<Ordens> Ordens { get; set; }
		public virtual DbSet<Pagos> Pagos { get; set; }



		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			// ============================
			// Llaves primaruas Definidas
			// ============================
			modelBuilder.Entity<Ciudades>()
			.HasKey(x => x.Id);

			modelBuilder.Entity<SolicitudesCiudades>()
				.HasKey(x => x.Id);

			modelBuilder.Entity<Solicitudes>()
				.HasKey(x => x.Id);

			modelBuilder.Entity<Yonkes>()
				.HasKey(x => x.Id);

			modelBuilder.Entity<YonkesCoberturas>()
				.HasKey(x => x.Id);

			modelBuilder.Entity<YonkesDispositivos>()
				.HasKey(x => x.Id);

			modelBuilder.Entity<SolicitudYonkes>()
				.HasKey(x => x.Id);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.HasKey(x => x.Id);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.HasAlternateKey(x => x.GuidId);

			modelBuilder.Entity<YonkesCalificaciones>(entity =>
			{
				entity.HasKey(x => x.Id);
				entity.HasIndex(x => x.GuidId)
					  .IsUnique();
				entity.HasIndex(x => new
				{
					x.CotizacionGuidId,
					x.UsuarioId
				})
				.IsUnique();
				entity.HasOne(x => x.Yonkes)
					  .WithMany()
					  .HasForeignKey(x => x.YonkeGuidId)
					  .HasPrincipalKey(x => x.GuidId)
					  .OnDelete(DeleteBehavior.Restrict);
				entity.HasOne(x => x.Solicitudes)
					  .WithMany()
					  .HasForeignKey(x => x.SolicitudGuidId)
					  .HasPrincipalKey(x => x.GuidId)
					  .OnDelete(DeleteBehavior.Restrict);
				entity.HasOne(x => x.Cotizacion)
					  .WithMany()
					  .HasForeignKey(x => x.CotizacionGuidId)
					  .HasPrincipalKey(x => x.GuidId)
					  .OnDelete(DeleteBehavior.Restrict);
			});




			// ============================
			// Configuración Solicitudes
			// ============================
			modelBuilder.Entity<Solicitudes>()
				.HasAlternateKey(x => x.GuidId)
				.HasName("AK_Solicitudes_GuidId");

			modelBuilder.Entity<Solicitudes>(entity =>
			{
				entity.HasKey(x => x.Id);
				entity.HasAlternateKey(x => x.GuidId);
				entity.Property(x => x.GuidId)
					  .IsRequired();
			});


			// Relación Solicitudes -> SolicitudesCiudades
			modelBuilder.Entity<SolicitudesCiudades>()
				.HasOne(x => x.Solicitudes)
				.WithMany(x => x.SolicitudesCiudades)
				.HasForeignKey(x => x.SolicitudGuidId)
				.HasPrincipalKey(x => x.GuidId)
				.OnDelete(DeleteBehavior.Cascade);


			// Relación Ciudades -> SolicitudesCiudades
			modelBuilder.Entity<SolicitudesCiudades>()
				.HasOne(x => x.Ciudades)
				.WithMany(x => x.SolicitudesCiudades)
				.HasForeignKey(x => x.CiudadId)
				.OnDelete(DeleteBehavior.Restrict);


			// ==========================================
			// SOLICITUDES IMAGENES
			// ==========================================

			modelBuilder.Entity<SolicitudesImagenes>(entity =>
			{
				entity.HasKey(x => x.Id);

				entity.HasOne(x => x.Solicitudes)
					  .WithMany(x => x.solicitudesImagenes)
					  .HasForeignKey(x => x.SolicitudGuidId)
					  .HasPrincipalKey(x => x.GuidId)
					  .OnDelete(DeleteBehavior.Cascade);
			});


			modelBuilder.Entity<SolicitudesHistorials>(entity =>
			{
				entity.HasKey(x => x.Id);
				entity.HasOne(x => x.Solicitudes)
					  .WithMany(x => x.solicitudesHistorials)
					  .HasForeignKey(x => x.SolicitudGuidId)
					  .HasPrincipalKey(x => x.GuidId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// ============================
			// Configuración Yonkes
			// ============================
			modelBuilder.Entity<Yonkes>()
				.HasAlternateKey(x => x.GuidId)
				.HasName("AK_Yonkes_GuidId");

			modelBuilder.Entity<YonkesCoberturas>()
				.HasOne(x => x.Yonkes)
				.WithMany(x => x.YonkesCoberturas)
				.HasForeignKey(x => x.YonkeGuidId)
				.HasPrincipalKey(x => x.GuidId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<YonkesCoberturas>()
				.HasOne(x => x.Ciudades)
				.WithMany(x => x.YonkesCoberturas)
				.HasForeignKey(x => x.CiudadId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<YonkesDispositivos>()
				.HasOne(x => x.Yonkes)
				.WithMany(x => x.YonkesDispositivos)
				.HasForeignKey(x => x.YonkeGuidId)
				.HasPrincipalKey(x => x.GuidId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SolicitudYonkes>()
				.HasOne(x => x.Solicitudes)
				.WithMany(x => x.SolicitudYonkes)
				.HasForeignKey(x => x.SolicitudGuidId)
				.HasPrincipalKey(x => x.GuidId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SolicitudYonkes>()
				.HasOne(x => x.Yonkes)
				.WithMany(x => x.SolicitudYonkes)
				.HasForeignKey(x => x.YonkeGuidId)
				.HasPrincipalKey(x => x.GuidId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.HasAlternateKey(x => x.GuidId)
				.HasName("AK_SolicitudCotizaciones_GuidId");

			modelBuilder.Entity<SolicitudCotizaciones>()
				.HasOne(x => x.SolicitudYonkes)
				.WithMany(x => x.SolicitudCotizaciones)
				.HasForeignKey(x => x.SolicitudYonkeGuidId)
				.HasPrincipalKey(x => x.GuidId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.HasOne(x => x.SolicitudCotizacionEstatus)
				.WithMany()
				.HasForeignKey(x => x.EstatusId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<SolicitudCotizacionesImagenes>()
				.HasOne(x => x.Cotizacion)
				.WithMany(x => x.SolicitudCotizacionesImagenes)
				.HasForeignKey(x => x.CotizacionGuidId)
				.HasPrincipalKey(x => x.GuidId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Ordens>(entity =>
			{
				entity.HasIndex(x => x.GuidId)
					.IsUnique();
				entity.HasIndex(x => x.CotizacionGuidId);
				entity.HasIndex(x => x.UsuarioId);
				entity.HasIndex(x => x.YonkeGuidId);
				entity.Property(x => x.PrecioPieza)
					.HasPrecision(18, 2);
				entity.Property(x => x.CostoEnvio)
					.HasPrecision(18, 2);
				entity.Property(x => x.TotalCliente)
					.HasPrecision(18, 2);
				entity.Property(x => x.BaseComision)
					.HasPrecision(18, 2);
				entity.Property(x => x.ComisionRefanetPorcentaje)
					.HasPrecision(5, 2);
				entity.Property(x => x.ComisionRefanetImporte)
					.HasPrecision(18, 2);
				entity.Property(x => x.ImporteYonke)
					.HasPrecision(18, 2);
			});

			modelBuilder.Entity<Pagos>(entity =>
			{
				entity.HasIndex(x => x.GuidId)
					.IsUnique();
				entity.HasIndex(x => x.OrdenGuidId);
				entity.HasIndex(x => x.CotizacionGuidId);
				entity.HasIndex(x => x.StripePaymentIntentId)
					.IsUnique()
					.HasFilter("[StripePaymentIntentId] IS NOT NULL");
				entity.HasIndex(x => x.StripeCheckoutSessionId)
					.IsUnique()
					.HasFilter("[StripeCheckoutSessionId] IS NOT NULL");
				entity.HasIndex(x => x.StripeEventId)
					.IsUnique()
					.HasFilter("[StripeEventId] IS NOT NULL");
				entity.Property(x => x.Importe)
					.HasPrecision(18, 2);
				entity.Property(x => x.ComisionRefanetPorcentaje)
					.HasPrecision(5, 2);
				entity.Property(x => x.ComisionRefanetImporte)
					.HasPrecision(18, 2);
				entity.Property(x => x.ImporteYonke)
					.HasPrecision(18, 2);
				entity.Property(x => x.StripeFee)
					.HasPrecision(18, 2);
				entity.Property(x => x.GananciaRefanet)
					.HasPrecision(18, 2);
			});


			modelBuilder.Entity<SolicitudCotizacionMensajes>(entity =>
			{
				entity.HasKey(x => x.Id);

				entity.Property(x => x.GuidId)
					.IsRequired();

				entity.Property(x => x.UsuarioId)
					.IsRequired()
					.HasMaxLength(450);

				entity.Property(x => x.Mensaje)
					.IsRequired()
					.HasMaxLength(2000);

				entity.HasOne(x => x.SolicitudCotizacion)
					.WithMany(x => x.SolicitudCotizacionMensajes)
					.HasForeignKey(x => x.SolicitudCotizacionGuidId)
					.HasPrincipalKey(x => x.GuidId)
					.OnDelete(DeleteBehavior.Cascade);
			});


			//Clietne Otp Sms 
			modelBuilder.Entity<ClienteOtps>(entity =>			{
				entity.ToTable("ClienteOtps");
				entity.HasKey(x => x.Id);
				entity.Property(x => x.UserId)
					.HasMaxLength(450)
					.IsRequired();
				entity.Property(x => x.PhoneNumber)
					.HasMaxLength(20)
					.IsRequired();
				entity.Property(x => x.CodigoHash)
					.HasMaxLength(128)
					.IsRequired();
				entity.Property(x => x.Ip)
					.HasMaxLength(45);
				entity.HasIndex(x => new
				{
					x.PhoneNumber,
					x.Activo,
					x.Usado
				});
			});


			//error se inserten dos registros del mismo yonke
			modelBuilder.Entity<SolicitudYonkes>()
				.HasIndex(x => new
				{
					x.SolicitudGuidId,
					x.YonkeGuidId
				}).IsUnique();


			//Indice Unico
			modelBuilder.Entity<YonkesCoberturas>()
			.HasIndex(x => new
			{
				x.YonkeGuidId,
				x.CiudadId
			}).IsUnique();

			modelBuilder.Entity<Yonkes>()
				.HasIndex(x => x.GuidId)
				.IsUnique();

			modelBuilder.Entity<Solicitudes>()
				.HasIndex(x => x.GuidId)
				.IsUnique();

			modelBuilder.Entity<SolicitudCotizaciones>()
				.HasIndex(x => x.GuidId)
				.IsUnique();

			//Cada yunke solo envie una cotizzacion
			modelBuilder.Entity<SolicitudCotizaciones>()
				.HasIndex(x => x.SolicitudYonkeGuidId)
				.IsUnique();


			modelBuilder.Entity<SolicitudYonkes>()
				.HasIndex(x => x.YonkeGuidId);

			modelBuilder.Entity<SolicitudYonkes>()
				.HasIndex(x => x.EstatusId);

			modelBuilder.Entity<SolicitudYonkes>()
				.HasIndex(x => x.SolicitudGuidId);




			//vALIDACIONES fLUEND
			modelBuilder.Entity<SolicitudCotizaciones>()
				.Property(x => x.MarcaId)
				.HasMaxLength(100);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.Property(x => x.NumeroParte)
				.HasMaxLength(100);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.Property(x => x.Comentarios)
				.HasMaxLength(1000);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.Property(x => x.Precio)
				.HasPrecision(10, 2);

			modelBuilder.Entity<SolicitudCotizaciones>()
				.Property(x => x.CostoEnvio)
				.HasPrecision(10, 2);

			//Token de Firebase para mesnajes
			modelBuilder.Entity<YonkesDispositivos>(entity =>
			{
				entity.Property(x => x.FirebaseToken)
					.HasMaxLength(1000)
					.IsUnicode();

				entity.Property(x => x.Plataforma)
					.HasMaxLength(50);

				entity.Property(x => x.Modelo)
					.HasMaxLength(100);
			});


		}


	}
}
