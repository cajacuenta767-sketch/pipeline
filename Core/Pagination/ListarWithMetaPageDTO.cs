namespace Core.Pagination
{
	public class ListarWithMetaPageDTO
	{
		public object[] data { get; set; }
		public Meta meta { get; set; }
	}
	public class Meta
	{
		public int page { get; set; }
		public int take { get; set; }
		public int itemCount { get; set; }
		public int pageCount { get; set; }
	}
}
