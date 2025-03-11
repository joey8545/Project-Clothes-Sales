namespace Project.DTO
{
    public class CommentDTO
    {
        public int Rating { get; set; }
        public int Oid { get; set; }
        public int Pid { get; set; }
        public string PSize { get; set; } 
        public string PColor { get; set; }
        public string CommentText { get; set; }
        public IFormFile Image1 { get; set; }
        public IFormFile Image2 { get; set; }
    }
}
