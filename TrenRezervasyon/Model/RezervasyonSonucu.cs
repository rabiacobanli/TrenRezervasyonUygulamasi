namespace TrenRezervasyon.Model
{
    public class RezervasyonSonucu
    {
        public bool RezervasyonYapilabilir { get; set; }
        public List<YerlesimAyrinti> YerlesimAyrinti { get; set; }
    }
}
