using Microsoft.AspNetCore.Mvc;
using TrenRezervasyon.Model;

namespace TrenRezervasyon.Controllers
{
    [ApiController]
    [Route("api/rezervasyon")]
    public class RezervasyonController : ControllerBase
    {
        [HttpPost]
        public IActionResult RezervasyonYap([FromBody] RezervasyonIstegi istek)
        {
            Tren tren = istek.Tren;
            int rezervasyonYapilacakKisiSayisi = istek.RezervasyonYapilacakKisiSayisi;
            bool kisilerFarkliVagonlaraYerlestirilebilir = istek.KisilerFarkliVagonlaraYerlestirilebilir;

            var rezervasyonSonucu = new RezervasyonSonucu
            {
                RezervasyonYapilabilir = true,
                YerlesimAyrinti = new List<YerlesimAyrinti>()
            };

            if(kisilerFarkliVagonlaraYerlestirilebilir)
            {
                foreach (var vagon in tren.Vagonlar)
                {
                    if (rezervasyonYapilacakKisiSayisi > 0 )
                    {
                        int bosKoltukAdet = vagon.Kapasite * 70 / 100 - vagon.DoluKoltukAdet;
                        int kisiSayisi = Math.Min(rezervasyonYapilacakKisiSayisi, bosKoltukAdet);
                        if(kisiSayisi > 0)
                        {
                            rezervasyonSonucu.YerlesimAyrinti.Add(new YerlesimAyrinti
                            {
                                VagonAdi = vagon.Ad,
                                KisiSayisi = kisiSayisi
                            });

                            vagon.DoluKoltukAdet += kisiSayisi;
                            rezervasyonYapilacakKisiSayisi -= kisiSayisi;
                        }
                    }
                }
                if (rezervasyonYapilacakKisiSayisi > 0)
                {
                    rezervasyonSonucu.RezervasyonYapilabilir = false;
                }
            }
            else
            {
                foreach (var vagon in tren.Vagonlar)
                {
                    if (((double)(vagon.DoluKoltukAdet + rezervasyonYapilacakKisiSayisi) / vagon.Kapasite) <= 0.7)
                    {
                        int kisiSayisi = Math.Min(rezervasyonYapilacakKisiSayisi, vagon.Kapasite - vagon.DoluKoltukAdet);
                        rezervasyonSonucu.YerlesimAyrinti.Add(new YerlesimAyrinti
                        {
                            VagonAdi = vagon.Ad,
                            KisiSayisi = kisiSayisi
                        });
                        vagon.DoluKoltukAdet += kisiSayisi;
                        rezervasyonYapilacakKisiSayisi -= kisiSayisi;
                        if (rezervasyonYapilacakKisiSayisi == 0)
                            break;
                    }
                }
                if (rezervasyonYapilacakKisiSayisi > 0)
                {
                    rezervasyonSonucu.RezervasyonYapilabilir = false;
                }
            }
            return Ok(rezervasyonSonucu);
        }
    }
}
