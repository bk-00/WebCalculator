using System.ComponentModel.DataAnnotations;

namespace WebCalculator.Models
{
    public class HomeViewModel
    {
        [Required]
        public string Expression { get; set; }
        public double Ans { get; set; }
        public string Message { get; set; }

        private bool validExp = true;
        public bool ValidExpression
        {
            get { return validExp; }
            set { validExp = value; }
        }

        public HomeViewModel()
        {
            Expression = string.Empty;
            Ans = 0;
            Message = string.Empty;
            validExp = true;
        }
    }
}
