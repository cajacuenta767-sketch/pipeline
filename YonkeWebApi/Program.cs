using Core.DTO.JWT;
using Core.DTO.Login.smsMasicos.Core.Configuration;
using Core.DTO.Stripe;
using Core.Interfaces.Auth;
using Core.Interfaces.Auth.AccesoDetalle;
using Core.Interfaces.Azure;
using Core.Interfaces.BuildSecurity;
using Core.Interfaces.Fijas;
using Core.Interfaces.Firebase;
using Core.Interfaces.JWT;
using Core.Interfaces.Login;
using Core.Interfaces.Login.AppleToken;
using Core.Interfaces.Login.AuthSoporte;
using Core.Interfaces.Login.ClienteOtp;
using Core.Interfaces.Login.UserDispotivos;
using Core.Interfaces.Login.Yonke;
using Core.Interfaces.Login_Cliente;
using Core.Interfaces.Login_Cliente.GoogleApple;
using Core.Interfaces.Negocio;
using Core.Interfaces.Negocio.Calificacion;
using Core.Interfaces.Negocio.Coberturas;
using Core.Interfaces.Negocio.Empresa;
using Core.Interfaces.Negocio.YunkeDispotivos;
using Core.Interfaces.Requests;
using Core.Interfaces.Requests.ciudades;
using Core.Interfaces.Requests.Estatus;
using Core.Interfaces.Requests.imagenes;
using Core.Interfaces.Requests.Solicitud;
using Core.Interfaces.RequestYonkes;
using Core.Interfaces.RequestYonkes.Cotizaciones;
using Core.Interfaces.RequestYonkes.CotizacionesImagenes;
using Core.Interfaces.RequestYonkes.CotizacionMessages;
using Core.Interfaces.RequestYonkes.OrdenesPago;
using Core.Interfaces.RequestYonkes.Solicitudes;
using Core.Interfaces.SingalR;
using Core.Interfaces.SmtpGmail;
using Core.Interfaces.Stripe;
using Core.Interfaces.Utilerias;
using Core.Interfaces.Utilerias.brand;
using Core.Interfaces.Utilerias.Citys;
using Core.Interfaces.Utilerias.Models;
using Core.Interfaces.Utilerias.States;
using Core.Models;
using Core.Services.Firebase;
using Core.Services.Google_Login;
using Core.Services.JWT;
using Core.Services.Login;
using Core.Services.Negocio.Empresa;
using Core.Services.Ordens;
using Core.Services.Requests;
using Core.Services.RequestYonkes;
using Core.Services.SendSmtpGmail;
using Core.Services.SignalR;
using Core.Services.smsMasivo;
using Core.Services.SolicitudCotizaciones;
using Core.Services.Soporte.AccesoDetalles;
using Core.Services.Stripe;
using Core.Services.Utilerias.brand;
using Core.Services.Utilerias.Citys;
using Core.Services.Utilerias.Models;
using Core.Services.Utilerias.States;
using Core.ServiciosAzure;
using Core.SignalRHub;
using FirebaseAdmin;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Infra.DataContext;
using Infra.Filters;
using Infra.Mappings;
using Infra.Repositorys.Login;
using Infra.Repositorys.Negocio;
using Infra.Repositorys.Requests;
using Infra.Repositorys.RequestYonkes;
using Infra.Repositorys.SeguridadUser;
using Infra.Repositorys.SendGrid;
using Infra.Repositorys.Soporte;
using Infra.Repositorys.Utilerias;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;



var builder = WebApplication.CreateBuilder(args);


// ==========================================
// Signal R
// ==========================================

builder.Services.AddSignalR();



// ==========================================
// Firebase Admin SDK
// ==========================================

var firebasePath = Path.Combine(
	builder.Environment.ContentRootPath,
	"Firebase",
	"firebase-service-account.json");

if (!File.Exists(firebasePath))
{
	throw new FileNotFoundException(
		"No se encontró el archivo de configuración de Firebase.",
		firebasePath);
}

if (FirebaseApp.DefaultInstance == null)
{
	var credential = GoogleCredential.FromFile(firebasePath);

	FirebaseApp.Create(new AppOptions
	{
		Credential = credential
	});
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();






//se implementa Cors para cualquier metodo y header
#region CORS Config anterior con Policy
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
	builder.AllowAnyOrigin()
		   .AllowAnyMethod()
		   .AllowAnyHeader();
}));
#endregion


#region Inject AppSettings
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
#endregion

#region Conexion BD Phosp
builder.Services.AddDbContext<AplicationDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionSmarter")));
#endregion

#region Identity  User and Role COP Coders
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
	//Password Definition
	opt.Password.RequireDigit = false;
	opt.Password.RequireLowercase = false;
	opt.Password.RequireNonAlphanumeric = false;
	opt.Password.RequireUppercase = false;
	opt.Password.RequiredLength = 4;
	//opt.Password.RequiredUniqueChars = 1;

	// Default Lockout settings.
	opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(185);
	opt.Lockout.MaxFailedAccessAttempts = 5;
	opt.Lockout.AllowedForNewUsers = true;

	//Required Email Confirmation
	opt.User.RequireUniqueEmail = true;
	opt.SignIn.RequireConfirmedEmail = true;

	//default desbloqueo para los nuevos usuarios
	opt.Lockout.AllowedForNewUsers = false;
}
).AddEntityFrameworkStores<AplicationDBContext>().AddDefaultTokenProviders();
#endregion

#region Creacion del token building securyri Video  FUNCIONA
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,

			ValidIssuer = jwt!.Issuer,
			ValidAudience = jwt.Audience,

			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(jwt.Key)),

			  // IMPORTANTE para [Authorize(Roles = "...")]
			RoleClaimType = ClaimTypes.Role,

			// IMPORTANTE para User.FindFirstValue(ClaimTypes.NameIdentifier)
			NameClaimType = ClaimTypes.NameIdentifier
		};
	});

//Una hora para reset password
builder.Services.Configure<DataProtectionTokenProviderOptions>(
	options =>
	{
		options.TokenLifespan = TimeSpan.FromHours(1);
	});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<IJwtService, JwtService>();
#endregion

#region servicio para el BuildSecurity 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IMailService, SendGridMailService>();
#endregion

#region Cookies Login 
builder.Services.ConfigureApplicationCookie(o => {
	o.ExpireTimeSpan = TimeSpan.FromDays(185);
	o.SlidingExpiration = true;
});
#endregion



#region Interfazes y servicios del sistema
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IYonkeservice, Yonkeservice>();
builder.Services.AddTransient<IYunkeCoberturaService, YunkeCoberturaService>();
builder.Services.AddTransient<IYunkeDispositivoService, YunkeDispositivoService>();
builder.Services.AddScoped<ISolicitudYonkeService, SolicitudYonkeService>();


builder.Services.AddTransient<IAccesoDetalleService, AccesoDetalleService>();
builder.Services.AddTransient<IStateService, EntidadService>();
builder.Services.AddTransient<ICiudadService, CiudadService>();
builder.Services.AddTransient<IMarcaService, MarcaService>();
builder.Services.AddTransient<IModeloService, ModelService>();


builder.Services.AddTransient<ISolicitudService, SolicitudService>();
builder.Services.AddTransient<ISolicitudCiudadesService, SolicitudCiudadesService>();
builder.Services.AddTransient<ISolicitudEstatusService, SolicitudEstatusService>();
builder.Services.AddTransient<ISolicitudesImagenesService, SolicitudesImagenesService>();

builder.Services.AddScoped<IYunkeCalificacionService, YunkeCalificacionService>();

builder.Services.AddScoped<IUsuarioService, AuthSoporte>();

//Cotizaciones

builder.Services.AddScoped<ICotizacionYonkeService, CotizacionYonkeService>();

//Mensajes
builder.Services.AddScoped<ISolicitudCotizacionMensajeService, SolicitudCotizacionMensajeService>();


//Login con Google  y Apple
builder.Services.AddScoped<IClienteAuthService, ClienteAuthService>();

//Otp Sms cliente
builder.Services.AddScoped<IClienteOtpService, ClienteOtpService>();
builder.Services.AddScoped<IClienteOtpRepository, ClienteOtpRepository>();

//Twulio Sms Service
//builder.Services.AddHttpClient<ISmsService, TwilioSmsService>();

//Sms Masivos
builder.Services.Configure<SmsMasivosSettings>(builder.Configuration.GetSection("SmsMasivos"));

//servicios de gmail correo
builder.Services.AddTransient<IMailSmtpService, SendSmtpGmailService>();    

builder.Services.AddHttpClient<ISmsService, SmsMasivosService>(
	client =>
	{
		client.BaseAddress =
			new Uri("https://api.smsmasivos.com.mx");

		client.Timeout =
			TimeSpan.FromSeconds(15);
	});

//Ordenes
builder.Services.AddScoped<IOrdenService, OrdenService>();

//Stripe pagos
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();


//Regsitro de token de apple para confiar
builder.Services.AddScoped<IAppleTokenService, AppleTokenService>();


//Login con yonkes Identity
builder.Services.AddScoped<IYonkeAuthService, YonkeAuthService>();


//Notificaciones signal
builder.Services.AddScoped<IChatNotificationService, SignalRNotificationService>();

#endregion


#region registro de la interfaz que utiliza el reposotirio generico, sustituye a lo comentado arriba
//registro del Unit of Work

builder.Services.AddScoped(typeof(IRepositrioYunke<>), typeof(YunkeRepository<>));
builder.Services.AddScoped<IYunkeCoberturaRepository, YunkeCoberturaRepository>();   //No generico


builder.Services.AddScoped(typeof(IRepositorioUtilerias<>), typeof(UtileriaRepository<>));

builder.Services.AddScoped(typeof(ISolicitudesRepository<>), typeof(SolicitudesRepository<>));


builder.Services.AddTransient<IUnitOfWorkSolicitudes, UnitOfWorkSolicitudes>();
builder.Services.AddTransient<IUnitOfWorkSolicitudYonkes, UnitOfWorkSolicitudesYonkes > ();



builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//Regsitro de Firebase
builder.Services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();

//Solicitudes Yonkes
builder.Services.AddScoped(typeof(ISolicitudesYonkeRepository), typeof(SolicitudYonkeRepository));

//Cotizaciones
builder.Services.AddScoped<ISolicitudCotizacionRepository, CotizacionYonkeRepository>();

//Mensajes
builder.Services.AddScoped<ISolicitudCotizacionMensajeRepository, SolicitudCotizacionMensajeRepository>();


//Usaurio Dispositivos
builder.Services.AddScoped<IUsuariosDispositivosService, UsuariosDispositivosService>();




#endregion


#region Registro de todos los UnitOfWork
builder.Services.AddTransient<IUnitOfWorkSoporte, UnitOfWorkSoporte>();
builder.Services.AddTransient<IUnitOfWorkUtilerias, UnitOfWorkUtilerias>();
builder.Services.AddTransient<IUnitOfWorkNegocio, UnitOfWorkNegocio>();
builder.Services.AddTransient<IUnitOfWorkSolicitudYonkes, UnitOfWorkSolicitudesYonkes>();

//Login de google
builder.Services.AddTransient<IUnitOfWorkCliente, UnitOfWorkClientes>();
#endregion

//AutoMapper
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

//contraseña manejo
builder.Services.Configure<PasswordOptions>(builder.Configuration.GetSection("PasswordOptions"));


//solucion al problema de core en SmarterASP
builder.Services.Configure<IISOptions>(options =>
{
	options.ForwardClientCertificate = false;
});

//registro Newtsoft y rompar la referencia circular
builder.Services.AddControllers(options =>
{
	//controla la exepciones de manera global
	options.Filters.Add<GlobalExeptionFilter>();
	
}).AddNewtonsoftJson(options =>
{
	options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
})
//validar el modelo de forma manual con Fluent Validation la mejor opcion
.ConfigureApiBehaviorOptions(option =>
{
	//option.SuppressModelStateInvalidFilter = true;
});


//Registro de los Servicios de Azure Storage para gaurdar archivos
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

//Regsitro del Uso del AutoMapper para la conversion de entidades de dominio a DTO
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());                         cancelado

//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


//Uso Global del Action Filter 
builder.Services.AddMvc(options =>
{
	//registro de los filtros a nivel de la aplicacion
	options.Filters.Add<ValidationFilter>();
	//registro de Fluent Api Validator para las validaciones 
}).AddFluentValidation(options =>
{
	options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});

//Pages reset password
builder.Services.AddRazorPages();

//registro de documentacion swagger
builder.Services.AddSwaggerGen(doc =>
{
	doc.SwaggerDoc("v1", new OpenApiInfo { Title = "Web Api Yonke", Version = "V1" });
	// para generar la documentacion con los comentarios del controlador
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	doc.IncludeXmlComments(xmlPath);

	doc.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header
	});

	doc.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[]{ }
					}
				});

});

//checar para que es
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Add InMemoryCache
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

//uso del swagger
app.UseSwagger();
//mandar la url con interfax de usuario desde la misma API
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Api Yonke V2");
	//arranque en la documentacion como pagina de inicio
	options.RoutePrefix = string.Empty;
});

app.UseRouting();

app.UseCors("MyPolicy");

app.UseHttpsRedirection();

app.UseStaticFiles();



//usar los JWT 
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	// add razor pages razor
	endpoints.MapRazorPages();
	endpoints.MapControllers();
});

app.MapControllers();

//Signal R
var signalRHubPath =
	builder.Configuration["SignalR:HubPath"]
	?? "/hubs/notificaciones";

app.MapHub<ChatHub>(signalRHubPath);



app.Run();



