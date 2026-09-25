using AutoMapper;
using Core.DTO.Empresas;
using Core.DTO.Solicitudes.Requests;
using Core.DTO.Utilerias.Ciudades;
using Core.DTO.Utilerias.Marcas;
using Core.DTO.Utilerias.Models;
using Core.DTO.Utilerias.States;
using Core.Entitys;

namespace Infra.Mappings
{
	public class AutoMapperProfile : Profile
	{		
		public AutoMapperProfile()
		{
			#region Auth acceso a empresas
			//CreateMap<AccesoDetalles, AccesoDetallesDTO>().ReverseMap();
			//CreateMap<Empresas, EmpresaDTO>().ReverseMap();
			#endregion



			CreateMap<Yonkes, YonkesListDTO>().ReverseMap();
			CreateMap<Yonkes, YunkeDTO>().ReverseMap();
			CreateMap<YonkesListDTO, YunkeDTO>();
			CreateMap<Yonkes, YonkesaveDTO>().ReverseMap();

			CreateMap<YonkesaveDTO, Yonkes>().ForMember(
				dest => dest.LogoUrl,
				opt => opt.Ignore()
			);

		

			CreateMap<Yonkes, YonkesaveDTO>()
			.ForMember(
				dest => dest.LogoUrl,
				opt => opt.Ignore()
			);

			CreateMap<Yonkes, YunkeUpdateInfoDTO>().ReverseMap();

			CreateMap<YonkesDispositivos, RegistrarDispositivoDto>().ReverseMap();


			#region Utilerias
			CreateMap<Entidades, EntidadesListDTO>().ReverseMap();

			CreateMap<Ciudades, CiudadesListDTO>().ReverseMap();

			CreateMap<Marcas, MarcasListDTO>().ReverseMap();
			CreateMap<Marcas, MarcaSaveDTO>().ReverseMap();
			CreateMap<Marcas, MarcaUpdateDTO>().ReverseMap();

			CreateMap<Modelos, ModelosListDTO>().ReverseMap();
			CreateMap<Modelos, ModeloSaveDTO>().ReverseMap();
			CreateMap<Modelos, ModeloUpdateDTO>().ReverseMap();
			#endregion



			CreateMap<Solicitudes, Solicitudes_Create_DTO>().ReverseMap();
		}
	}
}
