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

![Resim-Arayüz gösterimi](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran1.png)

## Gezgin Satıcı Problemi Hakkında Bilgi
Problem de amaç, bir satıcının bulunduğu şehirden başlayarak her şehre sadece bir kez uğradıktan sonra
başladığı şehre dönen en kısa turu bulmaktır. Şehirler arası gidişi kuş bakışı olarak kabul edersek aşağıdaki
örnek koordinat gösterimi ile problem tam olarak anlaşılacaktır.

![Resim – Başlangıç Rotası](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran2.png)

## Kullanılan Çözüm Yöntemleri
Rotalama işleminin yapılabilmesi için iki tane sezgisel yaklaşımdan yararlanılır. Bunlar: tavlama benzetimi
ve yasaklı arama yöntemleridir. Her iki yöntemide kıyaslayarak çözüm sonucunda kalite artırımı ve çözüm
süresinin kısalması hedeflenmiştir. Ancak hedeflerin tam olarak gerçekleşebilmesi için kullanıcının girdi
parametrelerini doğru şekilde ayarlaması gerekmektedir.
Örneğin 20 noktayı rotalamak için yasaklı arama yöntemi çalışırken tabu listesi uzunluğunun 40 olması
çözüm sürecine bir fayda sağlamaz ya da tavlama benzetimi kullanırken sıcaklık azaltma oranının 1 olması
üretilen komşuluk sayısını azaltacak ve boltzman sabiti ile kurulu rassallığı olumsuz etkiler ve bir süre
sonra lineer arama formuna dönüşebilir.
### Tavlama Benzetimi İle Çözüm Yaklaşımı

Tavlama, malzemeyi belirli bir süre (tavlama sıcaklığına kadar) ısıttıktan sonra, yavaş yavaş
soğutmaktır. Tavlama malzemeyi rahatlatmak, yumuşatmak ve iç yapıyı daha kullanılabilir hale
getirmek için yapılan ısıl işlemlerin geneline verilen addır.Tavlama Benzetimi, İniş Algoritmasının
(Descent Algorithm) iyileştirilmiş halidir.
#### İniş Algoritması
- Rasgele seçilen bir çözüm ile aramaya başlanır.
- Komşu çözüm oluşturulur ve amaç fonksiyonundaki değişim hesaplanır.
- Eğer amaç fonksiyonunda azalma söz konusu ise (minimisazyon problemi için), komşu çözüm mevcut çözüm olarak kabul edilir.
- Bu süreç, amaç fonksiyonunda mevcut çözümün hiçbir komşusu iyileşme sağlamayana
kadar devam eder ve algoritma yerel bir eniyi (local minimum/maximum) ile sonlanır.

Tavlama Benzetiminde, amaç fonksiyonunda artışa neden olabilecek komşu hareketler bazen kabul
edilerek yerel en iyi noktalarından kurtulmak mümkündür. • Amaç fonksiyonunda artışa neden
olabilecek bu komşu hareketin kabul edilip edilmemesi, rassal olarak belirlenmektedir.[1]

![İniş algoritması çalışma mantığı](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran3.png)

### Yasaklı Arama Yöntemi İle Çözüm Yaklaşımı

Tabu Arama Algoritması, optimizasyon problemlerinin çözümü için F.Glover tarafından geliştirilmiş
iteratif bir araştırma algoritmasıdır. Temel yaklaşım, son çözüme götüren adımın dairesel hareketler
yapmasını önlemek için bir sonraki döngüde tekrarın yasaklanması veya cezalandırılmasıdır. Böylece
yeni çözümlerin incelenmesiyle Tabu Arama algoritması, bölgesel en iyi çözümün daha ilerisinde
bulunan çözümlerin araştırılabilmesi için bölgesel-sezgisel araştırmaya kılavuzluk etmektedir. Tabu
Arama algoritmasının bölgesel optimalliği aşmak amacıyla kullandığı temel prensip, değerlendirme
fonksiyonu tarafından her iterasyonda en yüksek değerlendirme değerine sahip hareketin bir sonraki
çözümü oluşturmak amacıyla seçilmesine dayanmaktadır. Bunu sağlamak amacıyla bir tabu listesi
oluşturulur, tabu listesinin orijinal amacı önceden yapılmış bir hareketin tekrarından çok tersine
dönmesini önlemektir. Tabu listesi kronolojik bir yapıya sahiptir ve esnek bir hafıza yapısı kullanır.
Tabu arama algoritması her ne kadar istenmeyen noktaların işaretlenmesi olarak açıklanmış olsa da
daha cazip noktaların işaretlenmesi olarak ta kullanılır.
Yasaklı armayı açıklamak için aşağıdaki gibi bir gösterimden yararlanılabilir:

![](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran4.png)

Yukarıdaki ifadeyi açıklarsak; amaç fonksiyonu c(x) maliyet veya kar fonksiyonun en küçük veya en
büyük değerin aranmaktadır fakat bu aramada x vektörü ile belirtilen kısıtlamalara uyularak çözüme
ulaşılacaktır. Başka bir ifade ile her x elemanı bir hareketi temsil eder ve tüm hareketler X ile
gösterilmektedir. Ancak daha doğru bir varsayım x vektörlerinin TA bellek yapısı olarak kullanıldığıdır.
Böylece vektörde tutulan bellek değerine bağlı olarak çözüm aramada bazı hareketler tabu olarak kabul
edilip engellenecek, bazılarına ise daha fazla odaklanacaktır. X vektöründeki her bir hareket ise mevcut çözümün bir komşusunun seçimini temsil eder.

### Çözüm Yöntemlerinin Projede Kullanımı
Tüm çözüm yöntemlerinin kullanım sürecinde, kullanıcı varsayılan değerleri değiştirerek çözüm sürecine
yön verebilir. Kullanıcıya uygulama ilk kez çalıştırıldığında, rastgele oluşturulmuş bir başlangıç çözümü
verilir. Bu çözüm üzerinde rota araştırması yapılır.
Tavlama benzetimi için varsayılan değerler: Sıcaklık azaltma oranı 0.99, sıcaklık 100 ve iterasyon değeri
1000 olarak belirlenmiştir. Kullanıcı ilk çözüm isteğini gerçekleştirdiğinde, rastgele olarak üretilmiş olan başlangıç çözümü üzerinden araştırma yapılmaya, komşuluk üretimine başlanır. Ancak
kullanıcı, uygulamayı kapatmadan, birinci çözümden elde ettiği sonucun akabinde yeniden bir tavlama
benzetimi yöntemiyle çözüm araştırma isteği verirse uygulamaya, uygulama başlangıç olarak ilk isteğin
ürettiği sonucu başlangıç çözümü olarak kabul eder ve araştırmasını bu çözüm üzerinden devam ettirir.
Her isteğin ardından üretilen çözümde oranına dikkat edilmeksizin iyiye gitme olduğu göz önüne alınırsa,
tavlama benzetimi kullanılarak yapılan ard arda isteklerin merdiven etkisiyle daha da iyi sonuçlara gideceği
söylenebilir.
Yasaklı arama yönteminde, başlangıç çözümü her çözüm isteği alındığında rastgele olarak yeniden
oluşturulmuştur bu sebeple yukarıda bahsi geçem merdiven etkisini yasaklı arama yöntemi kullanılan
isteklerin çözüm sonuçlarında görmek mümkün değildir.
Yasaklı arama yöntemi kullanırken dikkat edilen en önemli nokta tabu listesinin uzunluğudur. Rotalama için kullanılan nokta sayısı varsayılan olarak 20 adettir ancak bu boyut artırılabilir ya da azaltılabilir. Bu sebeple 4 adet nokta için 5 adet tabu liste boyutu vermek doğru bir yaklaşım değildir. Belli bir çözüm süresinden ya da iterasyon sayısından sonra uygulamanın ısrarla aynı sonucu en iyi çözüm kabul etmesi durumu göz önünde bulundurularak, 10 000 iterasyonda bir tabu listesinden bir eleman çıkarılmıştır.

## Nokta Tanımlaması
Uygulama rotalama işlemini yapabilmek için temelde nokta tanımlamasına ihtiyaç duyar. Nokta
tanımlamasında düzlem, x y koordinat sistemi şeklinde modellenmiştir ve noktalar bu düzlemde
konumlandırılmıştır.
Her noktanın konum bilgisi ondalıklı sayılar olarak, 15.6 inc’lik bir bilgisayarın ekran boyutları göz önüne
alınarak statik olarak verilmiştir. Ancak nokta tanımlaması yapılırken yalnızca koordinat bilgisi kayıt altına
alınmamıştır. Her nokta için aşağıda belirtilen değerler tanımlanmıştır.

a) İsim
b) Öznitelik numarası
c) Koordinat bilgisi
d) Geçiş yapabileceği noktalar kümesi bilgileri tutulur.

Geçiş yapılabilecek noktalar kümesinin çözüm süresi boyunca noktanın yanında taşınmasının sebebi,
programlama açısından okuma yazma işleminin çok maliyetli olması sebebiyledir. Her noktanın koordinat değerleri bir kez okunur ve hafızada tutulur.

### Kromozom ve Komşuluk Tanımlaması
Çözüm sürecinin en önemli aşamasından biri de kromozom yapısıdır. Kromozom noktalardan oluşan sıralı
bir küme olarak modellenmiştir. Noktaların kromozom üzerindeki sıralaması, soldan sağa doğru geçiş
yönünü ifade eder.

a) Kromozom noktalar kümesinden oluştuğu için, kromozom kendi içinde her noktanın koordinat, isim,
numara ve geçiş yapılabilecek noktalar kümesi değerlerini yanında taşır. Dolayısıyla bir kromozom
üretildikten sonra yapılması gereken tek şey o kromozom üzerinde toplam mesafe maliyetinin
hesaplanmasıdır.

b) Tavlama benzetiminde her iterasyonda bir adet komşuluk üretilmiştir.

c) Yasaklı arama yaklaşımında her iterasyonda komşuluk sayısı parametresi kadar komşuluk üretilmiştir. 

d) Aslında her bir kromozom linked list yapıdadır. Kromozomu oluşturan noktalar rota sıralı tanımlandığı gibi fiziksel hafızada sıralı şekillerde tutulurlar. Bu yöntem programlama açısından fayda sağlamıştır.

e) Her komşuluk bilgisi rota ve mesafe maliyeti bilgisini taşır.
