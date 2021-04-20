## Yıldız Ver! :star:
Probleminizin çözümünü öğrenmek veya çözümü başlatmak için bu projeyi beğendiyseniz veya kullanıyorsanız, lütfen ona bir yıldız verin. Teşekkürler!
<hr>
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

Her noktanın konum bilgisi ondalıklı sayılar olarak, 15.6 inc’lik bir bilgisayarın ekran boyutları göz önüne alınarak statik olarak verilmiştir. Ancak nokta tanımlaması yapılırken yalnızca koordinat bilgisi kayıt altına alınmamıştır. Her nokta için aşağıda belirtilen değerler tanımlanmıştır.

- İsim

- Öznitelik numarası

- Koordinat bilgisi

- Geçiş yapabileceği noktalar kümesi bilgileri tutulur.

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

![Resim-Kromozom Yapısı](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran5.png)

Yasaklı arama yönteminde komşuluk kümesi kullanılmıştır. Bu küme kromozomlardan oluşmaktadır.
Kromozomlar üzerinde rastgele takas işlemi yapılarak komşulukların üretilmesi sağlanır. Komşuluk yapısı list
veri yapısı şeklindedir. Bu yapıyı düz bir liste yapmak gibi düşünebiliriz. Ancak komşuluk kümesinde hesaplama
işlemi yaparken rastgele bir yaklaşım değil sıralı bir seçim işlemi yapılmıştır. Komşuluk kümesinde bulunan
kromozomların mesafe maliyeti, kümede bulunduğu sıraya göre hesaplama işlemine tabi olmuştur.

### Başlangıç Çözümünün Oluşturulması
Toplam mesafesi hesaplanmak istenen noktaların belirlenmesinin ardından başlangıç çözümünün oluşturulması
aşamasına geçilir. Hesaplanmak istenen bütün noktalar bir torbaya atılmış gibi düşünülür ve torbadan rastgele
olarak sırasıyla seçim yaparak kromozom yapısına nokta eklemesi yapılır

![Resim-Başlangıç çözüm oluşturma işlem](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran6.png)

### Rotanın Mesafe Maliyetinin Hesaplanması

Her iki çözüm yöntemi için ortak olan aşama, oluşturulan rotanın mesafe maliyetinin hesaplanması
işlemidir. Rotayı oluşturan nokta tanımları xy koordinat düzlemin esas alınarak modellenmiştir. Bu
sebeple öklit formülü kullanılarak iki nokta arasındaki mesafe hesaplanmıştır. Sonrasında rota içinde
bulunan tüm mesafeler toplanarak, toplam mesafe maliyeti elde edilmiştir.
- Öklit formülünün proje içerisinde C# programlama dili kullanılarak uygulanması

![Resim-Başlangıç çözüm oluşturma işlem](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran7.png)

### Yasaklı Hareketin Kontrolü

Yasaklı arama algoritmasının çalışma mekanizmasında esas amaç bazı hareketlerin tekrarlanmasını
önlemektir. Tabu listesinin boyutu aşıldığı durumda FIFO kuralı uygulanır. Tabu listesine giren ilk
hareket tabu listesinden çıkarılır ve yasaklı olmaktan kurtulur.
Programlama aşamasında fark edilmiştir ki A noktasının B noktasıyla yer değiştirmesi demek aynı
zamanda B noktasının A noktasıyla yer değiştirmesi anlamına gelmektedir. Bu sebeple yalnızca A-B
yer değiştirmesi durumu değil aynı anda B-A yer değiştirmesi olayıda kontrol edilmiştir.

![Resim-Tabu hareket kontrolü](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran8.png)

- Tabu hareket kontrolünün C# programlama dili kullanılarak kontrol edilmesi

![](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran9.png)

### Programın Çalışma Adımları

Programın çalışması sıralı işlemlerin uygulanması şeklinde tanımlanmıştır. Kullancıının müdahalesi
yanızca sezgisel yöntemin seçilmesi ve seçim yapılan bu yönteme ilişkin parametre ayarlarının
yapıldığı kısımdır. Kullanıcı parametre ayarını yapmasa dahi seçilen araştırma yöntemi varsayılan
parametrelerle çalışacaktır.
Programın çalışma adımları sırasıyla aşağıdaki gibidir.

a) Program içerisinde hard-code olarak tanımlı 20 nokta kullanılarak problem denen noktalar
kümesi tanımlanır. Bu tanımlama noktaların isim, numara ve koordinat bilgilerinin belirtilen
CityPoint sınıfı içerisinde kurallı şekilde yazılması işlemidir.

![Resim-CityPoint sınıfının tanımlama bilgisi](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran10.png)

Nokta bilgilerinin nesnelerle temsil edilmesinin ardından, rastgele olarak başlangıç çözümü
oluşturulur.

c) Oluşturulan başlangıç çözümü kullanıcıya grafiksel arayüzle ve string sıralı gösterimiyle
sunulur.

d) Kullanıcı, arayüz aracılığıyla parametre değişimi yapmaz ise seçim yaptığı sezgisel yöntem,
program içerisinde yine hard-code olarak tanımlanmış olan varsayılan değerlerle çözüm arama
sürecini başlatılır. Parametreleri değiştirmesinin ardından yine bir sezgisel arama yöntemi
seçerek, kullanıcı çözüm arama sürecini başlatır.

e) Çözüm arama sürecinin tamamlanmasının ardından, bulunan iyileştirilmiş rota kullanıcıya
grafiksel arayüzle ve string sıralı gösterimle sunulur. Ayrıca kullanıcıya çözüm süresinin ne
kadar olduğu ile ilgili anlaşılır formatta bilgilendirme yapılır.

![Resim-Sezgisel yöntemlerden biriyle çözüm arama süreci başlatılmadan önce](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran11.png)

![Resim-Sezgisel yöntemlerden biriyle çözüm arama süreci başlatılmadan sonra](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran12.png)

### Programın Çalışma Performansını Artıran İşlemler
Programın, nokta tanımlaması, rota belirlenmesi ve kabul edilebilir bir rota düzenlemesinin
gerçekleştirimi yaparken çalışma süresinin katlanılabilir olması gerekmektedir. Ayrıca bulunan
çözümün kullanıcıya anlaşılır şekilde sunulması da önemli bir durumdur. Bu amaçlar için yapılan
işlemleri aşağıdaki gibi sıralanmıştır.

a) Her iki sezgisel araştırma yöntemi için ortak olan fonksiyonlar statik sınıflarda
tanımlanmışlardır. Örneğin, çözüm süresi her iki yöntem için kullanıcıya belli bir formatta
sunulmuştur. Bu formatın tanımlamasının yapıldığı method statik olarak tanımlanmıştır. </br>

b) Bir noktanın diğer noktalara olan geçiş matris bilgisinin her hesaplama işleminde okunması
tercih edilmemiştir yalnızca bir kez okuma işlemi yapılmıştır.</br>

c) Her iki sezgisel araştırma yöntemi aynı problemin noktalarından bir rota hazırlamaya çalıştığı
için, problemin oluşturulduğu bölüm statik sınıf içerisinde tanımlanmıştır.</br>

d) Kullanıcının bulunan rotayı doğru şekilde anlayabilmesi için, noktalar arası geçişler oklarla
gösterilmiştir. Bu şekilde bir string format hazırlanmış ve kullanıcıya sunulmuştur.</br>

e) Rotanın kullanıcı tarafında net gözlemlenebilmesi için, noktalar arasındaki mesafeye sadık
kalarak, küçültme ölçeklemesiyle rota çizilerek gösterim yapılmıştır.</br>

f) Sürdürülebilirlik açısından esnektir. Çözüm için kullanılan noktalar kümesi artırılabilir ya da
azaltılabililir.</br>

### Program Kısıtları
Programın geliştirme aşamasında akla ön görülemeyen, çözüm sürecinin kontrolü ve çözüm kalitesini
artırmaya yönelik bazı durumlar oluşmuştur. Bunlar aşağıda sıralanmıştır.

a) Her bir komşuluk rotası ve komşuluğun mesafe maliyeti, kullanılan sezgisel yöntem ile bu
yönteme ait parametrelerle kaydının tutulması yapılmamıştır. Örneğin tavlama method için bu
yöntem uygulanabilseydi, sıcaklık düşüşüne bağlı olarak çözüm kalitesi gözlemlenebilirdi.

b) Başlangıç çözümünün ardından yapılan iyileştirilmiş çözüm, ikinci isteğe başlangıç çözümü
verilerek araştırma verilebilmesi(daha önce bahsedilen merdiven etkisi) yalnızca tavlama
benzetimi için kodlanabilmiştir.

c) Her çözüm isteği, çözüm isteğinin hangi yöntem ve parametrelerle yapılmak istendiği, elde
edilen sonuç ve çözüm süresi gibi bilgiler kaydedilmemiştir. Dolayısıyla kıyaslama işlemi
program çalışırken kullanıcının gözlem yeteneğine bırakılmıştır.

### Programın Uygulama Sonuçları
Programın amacı verilen 20 adet noktanın, toplamda en kısa mesafeyi bulacak şekilde rotalanmasıdır.
Bu işlemi yaparken en hızlı şekilde yapması hedeflenmiştir. Bu sebeple de iki sezgisel arama
yaklaşımının kıyaslanması yoluna gitmiştir. Sonuçlar değerlendirilirken adaletli bir kıyaslama
yapılmaya gayret edilmiştir. Her iki yaklaşım için aşağıda belirtilen durumlar göz önüne alınarak
sonuçlar değerlendirilmiştir.

a) Kontrol edilen komşuluk sayısının her iki sezgisel yaklaşım içinde aynı olmasına gayret edilmiştir.

b) İterasyon değeri 1000 sayısıyla başlanmış önce 1000’er 1000’er artırılmış ardından 10000’er
10000’er artırım yoluna gidilmiştir.

c) Yasaklı arama yönteminde tabu listesi boyutu 3 olarak başlatılmış sırasıyla 7 ve 9 değerleri
denenmiştir.

d) Tavlama benzetimi için sıcaklık 100 olarak belirlenmiş bunun dışında bir değer için kontrol
yapılmamıştır.

e) Yasaklı arama yönteminde komşuluk değeri 12 olarak belirlenmiş ve bunun dışında bir değer için
kontrol yapılmamıştır.

![](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran13.png)

![](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran14.png)

![](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran15.png)

- Bulunan sonuçlar değerlendirildiğinde, her iki yönteminde başlangıç çözümü üzerinde iyileştirme
yaptığı gözle görülür şekilde ortadadır.
- Yöntemler kendi arasında kıyaslanacak olursa, tavlama benzetiminin yasaklı arama yöntemine göre
daha iyi sonuçlar verdiği gözlemlenmiştir.

### İdeal Çözüm Gösterimi

Program çalıştırılarak pek çok sonuç elde edilmiştir. Elde edilen sonuçlar içerisinde en kısa mesafe
uzunluğu 3690,06 km bulunmuştur. Ancak bu sonucun en iyi sonuç olduğu halan bilinmemektedir. Pek çok
kez daha durumun test edilmesi gerekmektedir. Ancak bu değerden daha iyi bir sonuç elde edilemediği gibi,
başlangıç çözümlerine göre en az %50 oranda iyileşme olan bir sonuçtur.
Rotanın başlangıç noktasının hangi nokta olduğunun bir önemi yoktur. Çünkü hangi noktadan başlanırsa
başlansın yine aynı noktaya varılacağı için çember etkisi vardır ve toplam mesafe uzunluğu fark
etmeyecektir. Bu sebeple birden fazla aynı sonuca sahip çözümler olduğu söylenebilir. Dolayısıyla aşağıda
verilen çözüm görseli, 3690,06 mesafe değerine sahip çözümlerden herhangi biridir denilebilir.

![Resim-İdeal Çözüm Örneği](https://github.com/NisanurBulut/TezRota/blob/master/Photos/anaEkran16.png)

- Çözüm Sonuç:3690,06 km
- Rota:R => J => D => O => C => M => F => H => P => N => T => S => A => U => I => E => B
=> G => L => K => R

### Proje Geliştirme Sürecini Gösteren TFS Adresi

Projenin geliştirme aşaması başlangıçtan bitimine kadar TFS üzerinden kontrol edilmiştir.
Link : https://nisanurb.visualstudio.com/TSP/_git/TSP

# Kaynakça
1. http://web.firat.edu.tr/iaydin/bmu579/bmu_579_bolum7.pdf
