using Project.Models;

namespace Project.DTO
{
    public class AdviceDetailDTO
    {
        public int OId { get; set; }

      
        public string OName { get; set; }

        public string OPhone { get; set; }

        public string OEmail { get; set; }

        public string OAddress { get; set; }

        public string Odate { get; set; }

        public string Ostatus { get; set; }

        public bool OPayment { get; set; }

        public List<AdviceOrderDetailDTO> orderdetail { get; set; }


    }
}
