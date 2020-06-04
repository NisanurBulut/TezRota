# TezRota
## Proje Konusu ve Kullanıcı Etkileşimi
Problem seti içinde belirtilen koordinat düzleminde tanımlı 20 adet noktanın başlangıç noktası fark
etmeksizin kendi içerisinde en kısa uzunluğa sahip olacak şekilde rotalanması işlemini
gerçekleştirmektir.
-  Rotalanması istenen nokta sayısı 20 olarak belirlenmiştir ancak, bu nokta sayısı esnektir. Nokta
sayısı artırılabilir ya da azaltılabilir.
-  Proje konusu aslında, gezgin satıcı problem modelidir.
Rotalama işlemi yapılırken, kıyaslama işlemi ön plana alınmaya çalışılmıştır. Bunu sağlamak için
kullanılan sezgisel yaklaşımlar birden fazla tutulmuştur. Ayrıca sezgisel yaklaşımların çalışması için
gereken parametre değerleri sabit tutulmamış ve kullanıcıdan alınabilecek şekilde tasarım yoluna
gidilmiştir.
Kullanıcıya sunulan tek bir ekran vardır. Kullanıcı bu tek ekrandan parametre ayarlamalarını yaparak
bir tane çözüm yöntemi seçer ve çözüm sürecini başlatır. İstenen parametreler kullanılacak olan sezgisel
yaklaşımın ihtiyaçlarına göre sıcaklık, iterasyon sayısı gibi input’larla şekillenir. Bu parametreler için
varsayılan değerler tanımlanmıştır.

<b> Kullanıcıdan İstenen Parametreler ve Varsayılan Değerleri </b>
a) Sıcaklık-100 <br>
b) İterasyon -1000<br>
c) Sıcaklık Azaltma Oranı-0.99<br>
d) Tabu Liste Uzunluğu-3<br>
e) Komşuluk Sayısı-12<br>

Ekranın sol üst köşesinde zaman bilgisi gösterimi yapılmıştır. Oluşturulan rotanın toplam mesafesine
göre kullanıcıdan alınan hız değeriyle, rotanın ne kadar sürede tamamlanacağı kullanıcıya sunulmuştur.
rotalama işleminin çözüm süresi ve rotanın sıralı dizilimi ekranda gösterilmektedir. Çözüm süresi,
yalnızca algoritmanın çözüm süresini ifade etmektir.

![Resim-Arayüz gösterimi](https://github.com/NisanurBulut/TezRota/tree/master/Photos/anaEkran1.png)