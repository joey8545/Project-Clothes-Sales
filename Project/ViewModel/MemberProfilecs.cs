namespace Project.ViewModel
{
    public class MemberProfilecs
    {
        public string Mname { get; set; }

        public int Mid { get; set; }

        public string Mgender  { get; set; }

        public string Memail { get; set; }

        public string Maddress { get; set; }

        public DateOnly Mbirthdays { get; set; }

        public string Mphone { get; set; }

        public IFormFile photoPath { get; set; }
    }
}
