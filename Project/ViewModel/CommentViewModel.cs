namespace Project.ViewModel
{
    public class CommentViewModel
    {
        public int ComID { get; set; }
        public int MID { get; set; }
        public string MName { get; set; }
        public int Rating { get; set; }
        public int OID { get; set; }
        public int PID { get; set; }
        public string PName { get; set; }
        public string PSize { get; set; }
        public string PColor { get; set; }
        public string Comment { get; set; }
        public DateTime ComCreateDate { get; set; }
        public string ComImage1 { get; set; }
        public string ComImage2 { get; set; }
    }
}
