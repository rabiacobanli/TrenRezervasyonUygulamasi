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
            int rezervasyonKisiSayisi = istek.RezervasyonYapilacakKisiSayisi;
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
                    if (rezervasyonKisiSayisi > 0 )
                    {
                        int bosKoltukSayisi = vagon.Kapasite * 70 / 100 - vagon.DoluKoltukAdet;
                        int kisiSayisi = Math.Min(rezervasyonKisiSayisi, bosKoltukSayisi);
                        if(kisiSayisi > 0)
                        {
                            rezervasyonSonucu.YerlesimAyrinti.Add(new YerlesimAyrinti
                            {
                                VagonAdi = vagon.Ad,
                                KisiSayisi = kisiSayisi
                            });

                            vagon.DoluKoltukAdet += kisiSayisi;
                            rezervasyonKisiSayisi -= kisiSayisi;
                        }
                    }
                }
                if (rezervasyonKisiSayisi > 0)
                {
                    rezervasyonSonucu.RezervasyonYapilabilir = false;
                }
            }
            else
            {
                foreach (var vagon in tren.Vagonlar)
                {
                    if (((double)(vagon.DoluKoltukAdet + rezervasyonKisiSayisi) / vagon.Kapasite) <= 0.7)
                    {
                        int kisiSayisi = Math.Min(rezervasyonKisiSayisi, vagon.Kapasite - vagon.DoluKoltukAdet);
                        rezervasyonSonucu.YerlesimAyrinti.Add(new YerlesimAyrinti
                        {
                            VagonAdi = vagon.Ad,
                            KisiSayisi = kisiSayisi
                        });
                        vagon.DoluKoltukAdet += kisiSayisi;

                        rezervasyonKisiSayisi -= kisiSayisi;

                        if (rezervasyonKisiSayisi == 0)
                            break;
                    }
                }
                if (rezervasyonKisiSayisi > 0)
                {
                    rezervasyonSonucu.RezervasyonYapilabilir = false;
                }
            }
            return Ok(rezervasyonSonucu);
        }
    }
}
