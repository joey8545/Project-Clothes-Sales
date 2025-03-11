namespace Project.ViewModel
{
    public class CheckDuplicateRequest

    {
        public string fieldName { get; set; }  // 例如 "Maccount" / "Memail" / "Mphone"
        public string fieldValue { get; set; }
    }
}
