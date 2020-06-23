using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    public bool spanish = false;
    public bool portuguese = false;
    public bool english = false;
	public string SelectedLanguage
	{
		get
		{
			if (spanish) return "Spanish";
			if (portuguese) return "Portuguese";
			if (english) return "English";
			return "";
		}
	}

    public GameObject subtitles;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        localizedText = new Dictionary<string, string>();
    }

    void Start()
    {
        LoadLocalizedText("localizedText_en.json");
    }

	//IEnumerator GetRequest(string uri)
	//{
		//UnityWebRequest uwr = UnityWebRequest.Get(uri);
		//yield return uwr.SendWebRequest();
		
		//WWW myW = new WWW( url );
		//yield return myW;
		/*if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
		}*/

	//}
	
    public void LoadLocalizedText(string fileName)
    {
		english = false;
		spanish = false;
		portuguese = false;
		
        localizedText.Clear();

        //if (Application.platform == RuntimePlatform.Android)
        //{
            //WWW reader = new WWW(Application.streamingAssetsPath + "/" + fileName);
            //while (!reader.isDone) { }
            //UnityWebRequest u = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileName);
            //u.SendWebRequest();
			//StartCoroutine(GetRequest(Application.streamingAssetsPath + "/" + fileName));
			
			//string jsonString = reader.text;// 
			string jsonString = "";
			
			if (fileName.Equals("localizedText_en.json"))
			{
				jsonString = @"{""items"": [
					{ ""key"": ""antarctica"", ""value"": ""Hey! Come in! "" },
					{ ""key"": ""antarctica01"", ""value"": ""... Hello? ... Sorry-"" },
					{ ""key"": ""antarctica02"", ""value"": ""We're still getting the kinks worked out of this new suit."" },
					{ ""key"": ""antarctica03"", ""value"": ""Let me know if this is working-"" },
					{ ""key"": ""antarctica04"", ""value"": ""I'm booting up the augmented reality overlay in your helmet now..."" },
					{ ""key"": ""antarctica05"", ""value"": ""Ok. Can you look up at the gaze point for me?"" },
					{ ""key"": ""antarctica06"", ""value"": ""Great! Now look at the one at your feet."" },
					{ ""key"": ""antarctica07"", ""value"": ""Alright! Everything seems to be in order."" },
					{ ""key"": ""antarctica08"", ""value"": ""Welcome to IceCube!"" },
					{ ""key"": ""antarctica09"", ""value"": ""I'm glad you could make it all the way"" },
					{ ""key"": ""antarctica10"", ""value"": ""down to Antarctica for this mission."" },
					{ ""key"": ""antarctica11"", ""value"": ""Before we send you off,"" },
					{ ""key"": ""antarctica12"", ""value"": ""let's brief you on why you're here:"" },
					{ ""key"": ""antarctica13"", ""value"": ""The IceCube Research Facility detects neutrino particles"" },
					{ ""key"": ""antarctica14"", ""value"": ""sent from deep in space."" },
					{ ""key"": ""antarctica15"", ""value"": ""I'll show you the sensors in your helmet overlay."" },
					{ ""key"": ""antarctica16"", ""value"": ""See that grid below the facility?"" },
					{ ""key"": ""antarctica17"", ""value"": ""Each dot is a sensor that detects light"" },
					{ ""key"": ""antarctica18"", ""value"": ""from a passing neutrino particle."" },
					{ ""key"": ""antarctica19"", ""value"": ""Look! It's just detected one now!"" },
					{ ""key"": ""antarctica20"", ""value"": ""This is great timing-"" },
					{ ""key"": ""antarctica21"", ""value"": ""we'll use the sensor data"" },
					{ ""key"": ""antarctica22"", ""value"": ""to pinpoint where the neutrino came from in outer space..."" },
					{ ""key"": ""antarctica23"", ""value"": ""annnnnd... done!"" },
					{ ""key"": ""antarctica24"", ""value"": ""Now your job is to follow this trajectory"" },
					{ ""key"": ""antarctica25"", ""value"": ""out into space and to find the source"" },
					{ ""key"": ""antarctica26"", ""value"": ""You're going to use your suit's Impossible Drive"" },
					{ ""key"": ""antarctica27"", ""value"": ""and the path of the neutrino."" },
					{ ""key"": ""antarctica28"", ""value"": ""All you have do to is"" },
					{ ""key"": ""antarctica29"", ""value"": ""look at the gaze point at the end of the path... Try it now."" },
					{ ""key"": ""voyager"", ""value"": ""Hello? ..."" },
					{ ""key"": ""voyager01"", ""value"": ""You still there?"" },
					{ ""key"": ""voyager02"", ""value"": ""Did you make it in one piece?"" },
					{ ""key"": ""voyager03"", ""value"": ""Take a second to look around and find your bearings-"" },
					{ ""key"": ""voyager04"", ""value"": ""it's probably pretty cool to be"" },
					{ ""key"": ""voyager05"", ""value"": ""further out in space than any other human has ever been!"" },
					{ ""key"": ""voyager06"", ""value"": ""Now you have a job to do."" },
					{ ""key"": ""voyager07"", ""value"": ""Follow the path of the neutrino"" },
					{ ""key"": ""voyager08"", ""value"": ""that was detected by IceCube"" },
					{ ""key"": ""voyager09"", ""value"": ""to discover the source."" },
					{ ""key"": ""voyager10"", ""value"": ""While we're waiting for the Impossible Drive to recharge,"" },
					{ ""key"": ""voyager11"", ""value"": ""let's go over some of the other features of your suit."" },
					{ ""key"": ""voyager12"", ""value"": ""If you look at your feet,"" },
					{ ""key"": ""voyager13"", ""value"": ""you can use the gaze points to"" },
					{ ""key"": ""voyager14"", ""value"": ""switch out your helmet's view."" },
					{ ""key"": ""voyager15"", ""value"": ""Go ahead-"" },
					{ ""key"": ""voyager16"", ""value"": ""look at your feet and switch to x-ray view."" },
					{ ""key"": ""voyager17"", ""value"": ""Pretty great, right?"" },
					{ ""key"": ""voyager18"", ""value"": ""Look around- check out the galaxy!"" },
					{ ""key"": ""voyager19"", ""value"": ""This is what the universe looks like when we see with x-rays."" },
					{ ""key"": ""voyager20"", ""value"": ""Your helmet is detecting x-rays"" },
					{ ""key"": ""voyager21"", ""value"": ""in the same way your eye would normally detect light."" },
					{ ""key"": ""voyager22"", ""value"": ""Can you look at Pluto for a second?"" },
					{ ""key"": ""voyager23"", ""value"": ""See how it's just a big black ball?"" },
					{ ""key"": ""voyager24"", ""value"": ""That's because x-rays don't pass through it."" },
					{ ""key"": ""voyager25"", ""value"": ""Now, let's switch to neutrino vision."" },
					{ ""key"": ""alright"", ""value"": ""Alright- now look back to Pluto:"" },
					{ ""key"": ""neutrino01"", ""value"": ""where'd it go?!"" },
					{ ""key"": ""neutrino02"", ""value"": ""Pluto seems to have disappeared!"" },
					{ ""key"": ""neutrino03"", ""value"": ""Your helmet is now only sensing neutrino particles."" },
					{ ""key"": ""neutrino04"", ""value"": ""Neutrinos pass through just about everything,"" },
					{ ""key"": ""neutrino05"", ""value"": ""Even whole planets!"" },
					{ ""key"": ""neutrino06"", ""value"": ""It's like Pluto doesn't even exist to them!"" },
					{ ""key"": ""neutrino07"", ""value"": ""When you're ready,"" },
					{ ""key"": ""neutrino08"", ""value"": ""Look to the gaze point at the end of the neutrino path and you'll follow the particle that was detected by IceCube."" },
					{ ""key"": ""journey"", ""value"": ""We're getting some... pretty intense readings."" },
					{ ""key"": ""journey01"", ""value"": ""You're... really far out in space."" },
					{ ""key"": ""journey02"", ""value"": ""Ok- time to brief you with the details of your mission."" },
					{ ""key"": ""journey03"", ""value"": ""As you've seen, we've given your suit the ability"" },
					{ ""key"": ""journey04"", ""value"": ""to see in three different ways:"" },
					{ ""key"": ""journey05"", ""value"": ""Visible light, x-ray vision, and neutrino detection."" },
					{ ""key"": ""journey06"", ""value"": ""The first two have been used for decades to look out into space."" },
					{ ""key"": ""journey07"", ""value"": ""But if we want to see really far,"" },
					{ ""key"": ""journey08"", ""value"": ""Neutrinos are the only thing that will work."" },
					{ ""key"": ""journey09"", ""value"": ""That's why we need the IceCube observatory."" },
					{ ""key"": ""journey10"", ""value"": ""The arrays of sensors in Antarctica"" },
					{ ""key"": ""journey11"", ""value"": ""allow us to detect neutrinos from deep space."" },
					{ ""key"": ""journey12"", ""value"": ""That helps us map out parts of the universe"" },
					{ ""key"": ""journey13"", ""value"": ""invisible to other telescopes."" },
					{ ""key"": ""journey14"", ""value"": ""The question you have to answer is:"" },
					{ ""key"": ""journey15"", ""value"": ""What sent the neutrino that"" },
					{ ""key"": ""journey16"", ""value"": ""IceCube detected back at Earth?"" },
					{ ""key"": ""journey17"", ""value"": ""When you find the source at the end of your journey,"" },
					{ ""key"": ""journey18"", ""value"": ""you'll need to collect data from it"" },
					{ ""key"": ""journey19"", ""value"": ""using each of the three methods we've given you."" },
					{ ""key"": ""journey20"", ""value"": ""You'll use your visible light,"" },
					{ ""key"": ""journey21"", ""value"": ""x-ray, and neutrino view to collect these readings."" },
					{ ""key"": ""journey22"", ""value"": ""Ok. Things might get dicey going forward."" },
					{ ""key"": ""journey23"", ""value"": ""Good luck."" },
					{ ""key"": ""hi"", ""value"": ""Hello?"" },
					{ ""key"": ""ask"", ""value"": ""Do you read me?"" },
					{ ""key"": ""discover"", ""value"": ""You've discovered a black hole!"" },
					{ ""key"": ""scan"", ""value"": ""You need to scan it with each"" },
					{ ""key"": ""scan01"", ""value"": ""of your vision modules quickly!"" },
					{ ""key"": ""getout"", ""value"": ""Then get OUT of there!"" },
					{ ""key"": ""light"", ""value"": ""Make sure you've selected visibile light vision-"" },
					{ ""key"": ""collect"", ""value"": ""look at the black hole and collect visible light readings!"" },
					{ ""key"": ""collect01"", ""value"": ""Look up at the black hole, and collect the x-ray readings!"" },
					{ ""key"": ""collect02"", ""value"": ""Look back at the black hole, and collect the neutrino readings!"" },
					{ ""key"": ""nice"", ""value"": ""You did it! We have the data!"" },
					{ ""key"": ""path"", ""value"": ""Now follow the neutrino path back to Earth! HURRY!"" },
					{ ""key"": ""wow"", ""value"": ""Wow! You did it!"" },
					{ ""key"": ""wow01"", ""value"": ""I... can't believe you're alive!"" },
					{ ""key"": ""congrats"", ""value"": ""....I mean, umm.. congratulations, agent!"" },
					{ ""key"": ""source"", ""value"": ""It looks like the source"" },
					{ ""key"": ""source01"", ""value"": ""of the neutrino particle we detected with IceCube"" },
					{ ""key"": ""source02"", ""value"": ""was a black hole!"" },
					{ ""key"": ""bh"", ""value"": ""Black holes are one of the strangest,"" },
					{ ""key"": ""blackhole01"", ""value"": ""most extreme objects in the whole universe!"" },
					{ ""key"": ""blackhole02"", ""value"": ""Did you know that black holes can have the mass"" },
					{ ""key"": ""sn"", ""value"": ""of several million suns?"" },
					{ ""key"": ""blackhole03"", ""value"": ""One spoonful of black hole could weigh as much"" },
					{ ""key"": ""blackhole04"", ""value"": ""as a whole planet!"" },
					{ ""key"": ""blackhole05"", ""value"": ""They also emit high energy neutrinos"" },
					{ ""key"": ""blackhole06"", ""value"": ""that travel millions of lightyears back to Earth."" },
					{ ""key"": ""blackhole07"", ""value"": ""It would have gone totally unnoticed"" },
					{ ""key"": ""blackhole08"", ""value"": ""if it weren't for the scientists at IceCube."" },
					{ ""key"": ""blackhole09"", ""value"": ""Black holes are very hard to detect-"" },
					{ ""key"": ""blackhole10"", ""value"": ""well- because they're black!"" },
					{ ""key"": ""blackhole11"", ""value"": ""It's impossible to see something black on a black background of space!"" },
					{ ""key"": ""thankyou"", ""value"": ""Fortunately, IceCube has found a way to observe them using neutrinos!"" },
					{ ""key"": ""end"", ""value"": ""Well, that's mission complete on our end."" },
					{ ""key"": ""bye"", ""value"": ""Until next time!"" },
					{ ""key"": ""pluto"", ""value"": ""PLUTO"" },
					{ ""key"": ""earth"", ""value"": ""EARTH"" },
					{ ""key"": ""milkyway"", ""value"": ""MILKY WAY"" },
					{ ""key"": ""visible"", ""value"": ""VISIBLE"" },
					{ ""key"": ""xray"", ""value"": ""X-RAY"" },
					{ ""key"": ""neutrino"", ""value"": ""NEUTRINO"" },
					{ ""key"": ""blackhole"", ""value"": ""BLACK HOLE"" },
					{ ""key"": ""sun"", ""value"": ""SUN"" },
					{ ""key"": ""map"", ""value"": ""Solar System"" },
					{ ""key"": ""map01"", ""value"": ""Local Group"" },
					{ ""key"": ""alert"", ""value"": ""ALERT"" },
					{ ""key"": ""Ice"", ""value"": ""Ice"" },
					{ ""key"": ""Voyager"", ""value"": ""Voyager"" },
					{ ""key"": ""Nothing"", ""value"": ""Nothing"" },
					{ ""key"": ""Extreme"", ""value"": ""Extreme"" },
					{ ""key"": ""Credits"", ""value"": ""Credits"" },
					{ ""key"": ""Earth"", ""value"": ""Earth"" },
                    { ""key"": ""start"", ""value"": ""START"" },
                    { ""key"": ""language"", ""value"": ""LANGUAGE"" },
					{ ""key"": ""c0"", ""value"": "" "" },
					{ ""key"": ""c1"", ""value"": "" "" },
					{ ""key"": ""c2"", ""value"": ""Exploring the Universe from Antarctica"" },
					{ ""key"": ""c3"", ""value"": "" "" },
					{ ""key"": ""c4"", ""value"": ""Supported by the National Science Foundation Office of Polar Programs"" },
					{ ""key"": ""c5"", ""value"": ""Award #1612504: Exploring the Universe from Antarctica"" },
					{ ""key"": ""c6"", ""value"": ""Produced by FIELD DAY"" },
					{ ""key"": ""c7"", ""value"": ""in collaboration with"" },
					{ ""key"": ""c8"", ""value"": ""Wisconsin Institute for Discovery: Virtual Environments group,"" },
					{ ""key"": ""c9"", ""value"": ""and Wisconsin IceCube Particle Astrophysics Center (WIPAC)"" },
					{ ""key"": ""c10"", ""value"": ""PI: Kevin Ponto"" },
					{ ""key"": ""c11"", ""value"": ""Producer: David Gagnon"" },
					{ ""key"": ""c12"", ""value"": ""Engineer: Phil Dougherty"" },
					{ ""key"": ""c13"", ""value"": ""Development: Katherine Ceballos, Simon Smith, Ross Tredinnick"" },
					{ ""key"": ""c14"", ""value"": ""Art Direction: Sarah Gagnon"" },
					{ ""key"": ""c15"", ""value"": ""Art / UX: Eric Lang"" },
					{ ""key"": ""c16"", ""value"": ""3d Modeling: Eric Peterson"" },
					{ ""key"": ""c17"", ""value"": ""Sound and Music: Andrew Fitzpatrik"" },
					{ ""key"": ""c18"", ""value"": ""Writing: Lindy Biller"" },
					{ ""key"": ""c19"", ""value"": ""WIPAC - Silvia Bravo-Gallart, James Madsen"" },
					{ ""key"": ""c20"", ""value"": ""A product of UW-Madison & IceCube"" },
					{ ""key"": ""c21"", ""value"": """" }]}";
			}
			else if(fileName.Equals("localizedText_es.json"))
			{
			jsonString = @"{""items"": [
				{ ""key"": ""antarctica"", ""value"": ""¡Ey! ¿Estás ahí?"" },
				{ ""key"": ""antarctica01"", ""value"": ""... ¿Hola? ..."" },
				{ ""key"": ""antarctica02"", ""value"": ""Perdón, todavía estamos tratando de arreglar algunos .."" },
				{ ""key"": ""antarctica03"", ""value"": "".. pequeños defectos del nuevo traje. "" },
				{ ""key"": ""test"", ""value"": ""Dime si funciona"" },
				{ ""key"": ""antarctica04"", ""value"": ""Ahora estoy encendiendo la capa de realidad aumentada en tu casco ..."" },
				{ ""key"": ""antarctica05"", ""value"": ""Ok. ¿Puedes mirar hacia arriba y buscar el punto de mira?"" },
				{ ""key"": ""antarctica06"", ""value"": ""¡Genial! Ahora mira hacia el que está a tus pies."" },
				{ ""key"": ""antarctica07"", ""value"": ""¡Perfecto! Todo parece estar en orden."" },
				{ ""key"": ""antarctica08"", ""value"": ""¡Bienvenido a IceCube!"" },
				{ ""key"": ""antarctica09"", ""value"": ""Me alegro de que hayas podido llegar"" },
				{ ""key"": ""antarctica10"", ""value"": ""hasta la Antártida para esta misión."" },
				{ ""key"": ""antarctica11"", ""value"": ""Antes de que te mandemos a tu misión,"" },
				{ ""key"": ""antarctica12"", ""value"": ""deja que te expliquemos por que estás aquí:"" },
				{ ""key"": ""antarctica13"", ""value"": ""El Observatorio de Neutrinos IceCube detecta neutrinos que llegan"" },
				{ ""key"": ""antarctica14"", ""value"": ""desde lo profundo del espacio."" },
				{ ""key"": ""antarctica15"", ""value"": ""Te enseñaré los sensores en la cubierta de tu casco. "" },
				{ ""key"": ""antarctica16"", ""value"": ""¿Ves esa cuadrícula detrás del edificio azul?"" }, 
				{ ""key"": ""antarctica17"", ""value"": ""Cada punto es un sensor que detecta luz"" },
				{ ""key"": ""antarctica18"", ""value"": ""de un neutrino pasajero."" },
				{ ""key"": ""antarctica19"", ""value"": ""¡Mira! ¡Acaba de detectar una ahora!"" },
				{ ""key"": ""antarctica20"", ""value"": ""Este es un buen momento-"" },
				{ ""key"": ""antarctica21"", ""value"": ""usaremos los datos de Ice Cube"" },
				{ ""key"": ""antarctica22"", ""value"": ""para determinar de dónde vino este neutrino"" },
				{ ""key"": ""antarctica23"", ""value"": ""desde el espacio exterior..."" },
				{ ""key"": ""antarctica24"", ""value"": ""yyyy... ¡hecho! "" },
				{ ""key"": ""antarctica25"", ""value"": ""Ahora tu trabajo es seguir esta trayectoria"" },
				{ ""key"": ""antarctica26"", ""value"": ""hacia el espacio y encontrar la fuente."" },
				{ ""key"": ""antarctica27"", ""value"": ""Vas a usar la Impulsión Imposible de tu traje"" },
				{ ""key"": ""antarctica28"", ""value"": ""y la trayectoria del neutrino-"" },
				{ ""key"": ""antarctica29"", ""value"": ""Todo lo que tienes que hacer es mirar hacia"" },
				{ ""key"": ""antarctica30"", ""value"": ""el punto de la mira al final de la trayectoria... "" },
				{ ""key"": ""antarctica31"", ""value"": ""Intentalo ahora"" },
				{ ""key"": ""voyager"", ""value"": ""¿Hola? ..."" },
				{ ""key"": ""voyager01"", ""value"": ""¿Sigues ahí?"" },
				{ ""key"": ""voyager02"", ""value"": ""¿Llegaste en una pieza?"" },
				{ ""key"": ""voyager03"", ""value"": ""Echa un segundo vistazo a tu alrededor y oriéntate-"" },
				{ ""key"": ""voyager04"", ""value"": ""¡Es genial estar más lejos en el espacio"" },
				{ ""key"": ""voyager05"", ""value"": ""de lo que cualquier otro humano ha estado!"" },
				{ ""key"": ""voyager06"", ""value"": ""Ahora tienes un trabajo por hacer."" },
				{ ""key"": ""voyager07"", ""value"": ""Sigue la trayectoria del neutrino"" },
				{ ""key"": ""voyager08"", ""value"": ""detectado por IceCube para descubrir la fuente."" },
				{ ""key"": ""voyager10"", ""value"": ""Mientras esperas que se cargue la Impulsión Imposible,"" },
				{ ""key"": ""voyager11"", ""value"": ""vamos a repasar algunas funciones de tu traje."" },
				{ ""key"": ""voyager12"", ""value"": ""Si miras a tus pies,"" },
				{ ""key"": ""voyager13"", ""value"": ""puedes usar los puntos de mira para"" },
				{ ""key"": ""voyager14"", ""value"": ""cambiar la visión de tu casco."" },
				{ ""key"": ""voyager15"", ""value"": ""Adelante-"" },
				{ ""key"": ""voyager16"", ""value"": ""Mira abajo hacia tus pies y cambia la visión a rayos X."" },
				{ ""key"": ""voyager17"", ""value"": ""Bueno, ¿verdad?"" },
				{ ""key"": ""voyager18"", ""value"": ""¡Mira a tu alrededor –"" },
				{ ""key"": ""voyager19"", ""value"": ""échale un vistazo a la galaxia! "" },
				{ ""key"": ""voyager20"", ""value"": ""Así es como se ve el universo cuando lo miramos con rayos X. "" },
				{ ""key"": ""voyager21"", ""value"": ""Tu casco está detectando rayos X"" },
				{ ""key"": ""voyager22"", ""value"": ""de la misma manera que tu ojo"" },
				{ ""key"": ""voyager23"", ""value"": ""normalmente detectaría la luz."" },
				{ ""key"": ""voyager24"", ""value"": ""¿Puedes mirar a Pluto por un segundo?"" },
				{ ""key"": ""voyager25"", ""value"": ""¿Ves como es una gran bola negra?"" },
				{ ""key"": ""voyager26"", ""value"": ""Esto se debe a que los rayos X no pasan a través suyo."" },
				{ ""key"": ""voyager27"", ""value"": ""Ahora cambia a la visión con neutrinos."" },
				{ ""key"": ""alright"", ""value"": ""Bien- Ahora mira de nuevo a Pluto:"" },
				{ ""key"": ""neutrino01"", ""value"": ""¡¿A dónde se fue?!"" },
				{ ""key"": ""neutrino02"", ""value"": ""¡Parece como que Pluto desapareció!"" },
				{ ""key"": ""neutrino03"", ""value"": ""Tu casco ahora solo detecta neutrinos."" },
				{ ""key"": ""neutrino04"", ""value"": ""Los neutrinos pasan a través de casi todo,"" },
				{ ""key"": ""neutrino05"", ""value"": ""hasta de planetas enteros."" },
				{ ""key"": ""neutrino06"", ""value"": ""¡Es como si Pluto ni existiera para ellos!"" },
				{ ""key"": ""neutrino07"", ""value"": ""Cuando estés listo,"" },
				{ ""key"": ""neutrino08"", ""value"": ""mira al punto de la mira al final de la trayectoria del neutrino"" },
				{ ""key"": ""neutrino09"", ""value"": ""y seguiras la particula detectada en IceCube"" },
				{ ""key"": ""journey"", ""value"": ""Nos están llegando algunos.. resultados bastante intensos."" },
				{ ""key"": ""journey0"", ""value"": ""Estás... muy lejos en el espacio."" },
				{ ""key"": ""journey01"", ""value"": ""Tiempo para informarte de los detalles de tu misión."" },
				{ ""key"": ""journey02"", ""value"":  ""Como has visto, "" },
				{ ""key"": ""journey03"", ""value"": ""le hemos dado a tu traje la habilidad de ver en tres diferentes maneras:"" },
				{ ""key"": ""journey030"", ""value"": ""luz visible, visión de rayos X, y detección de neutrinos."" },
				{ ""key"": ""journey04"", ""value"": ""Las primeras dos se han usado durante décadas para observar el espacio."" },
				{ ""key"": ""journey05"", ""value"": ""Pero si queremos de VERDAD ver lejos,"" },
				{ ""key"": ""journey06"", ""value"": ""los neutrinos son los únicos que funcionan."" },
				{ ""key"": ""journey07"", ""value"": ""Por eso necesitamos el observatorio de IceCube."" },
				{ ""key"": ""journey08"", ""value"": ""Esa matriz de sensores en la Antártida"" },
				{ ""key"": ""journey09"", ""value"": ""nos permite detectar neutrinos desde el espacio sideral,"" },
				{ ""key"": ""journey10"", ""value"": ""cosa que nos ayuda a cartografiar"" },
				{ ""key"": ""journey11"", ""value"": ""partes del universo invisibles para otros telescopios."" },
				{ ""key"": ""journey12"", ""value"": ""La pregunta que tienes que responder es"" },
				{ ""key"": ""journey13"", ""value"": ""¿Qué creo el neutrino que IceCube detectó en la Tierra?"" },
				{ ""key"": ""journey14"", ""value"": ""Cuando encuentres la fuente al final de tu viaje,"" },
				{ ""key"": ""journey15"", ""value"": ""necesitarás tomar datos de la fuente"" },
				{ ""key"": ""journey16"", ""value"": ""usando cada uno de los tres métodos que te hemos dado."" },
				{ ""key"": ""journey17"", ""value"": ""Usarás la luz visible, los rayos X y los neutrino"" },
				{ ""key"": ""journey18"", ""value"": ""para reunir estas tres lecturas."" },
				{ ""key"": ""journey19"", ""value"": ""OK, las cosas podrían ponerse peligrosas de ahora en adelante."" },
				{ ""key"": ""journey20"", ""value"": ""Buena suerte."" },
				{ ""key"": ""hi"", ""value"": ""¿Hola?"" },
				{ ""key"": ""ask"", ""value"": ""¿Me escuchas?"" },
				{ ""key"": ""discover"", ""value"": ""¡Has descubierto un agujero negro!"" },
				{ ""key"": ""scan"", ""value"":  ""Tienes que escanearlo con cada módulo de visión RÁPIDAMENTE."" },
				{ ""key"": ""scan01"", ""value"":  ""¡Después sal de ahí cuando antes!"" },
				{ ""key"": ""getout"", ""value"": ""¡Asegúrate que has seleccionado la visión de luz visible-"" },
				{ ""key"": ""collect"", ""value"": ""mira al agujero negro y toma datos con luz visible!"" },
				{ ""key"": ""collect01"", ""value"": ""¡Mira arriba hacia el agujero negro para tomar los datos con rayos X!"" },
				{ ""key"": ""collect02"", ""value"": ""¡Mira arriba hacia el agujero negro para tomar datos con neutrinos!"" },
				{ ""key"": ""nice"", ""value"": ""¡Lo lograste! ¡Tenemos los datos!"" },
				{ ""key"": ""path"", ""value"": ""¡Ahora sigue la trayectoria del neutrino de regreso a la Tierra!"" },
				{ ""key"": ""fast"", ""value"": ""Rapido, rapido."" },
				{ ""key"": ""wow"", ""value"": ""¡Guau! ¡Lo lograste!"" },
				{ ""key"": ""wow01"", ""value"": ""Yo.. ¡Yo no puedo creer que estés vivo! Quiero decir.."" },
				{ ""key"": ""congrats"", ""value"": ""felicidades, agente!"" },
				{ ""key"": ""source"", ""value"": ""¡Parece que la fuente del neutrino que detectamos con IceCube era un agujero negro!"" },
				{ ""key"": ""source01"", ""value"": ""¡Los agujeros negros son unos de los objetos más raros y más extremos del universo!"" },
				{ ""key"": ""source02"", ""value"": ""¿Sabías que los agujeros negros pueden tener una masa de"" },
				{ ""key"": ""bh"", ""value"": ""algunos millones de soles? "" },
				{ ""key"": ""blackhole01"", ""value"": ""¡Una cucharada de un agujero negro podría pesar tanto como"" },
				{ ""key"": ""blackhole02"", ""value"": ""un planeta entero!"" },
				{ ""key"": ""sn"", ""value"": ""Emiten neutrinos de muy alta energía que viajan"" },
				{ ""key"": ""blackhole03"", ""value"": ""durante millones de años luz hasta la Tierra."" },
				{ ""key"": ""blackhole04"", ""value"": ""Hubiese pasado completamente desapercibido"" },
				{ ""key"": ""blackhole05"", ""value"": ""si no fuera por los científicos en IceCube."" },  
				{ ""key"": ""blackhole06"", ""value"": ""Los agujeros negros son muy difíciles de detectar-"" },
				{ ""key"": ""blackhole07"", ""value"": ""El motivo es que están muy lejos y casi nada puede escaparse de ellos."" },
				{ ""key"": ""blackhole08"", ""value"": ""Afortunadamente"" },
				{ ""key"": ""blackhole09"", ""value"": ""IceCube ha encontrado una forma de estudiarlos usando neutrinos"" },
				{ ""key"": ""blackhole10"", ""value"": ""Bueno, por nuestra parte, misión completa. ¡Hasta la próxima!"" },
				{ ""key"": ""blackhole11"", ""value"": """" },
				{ ""key"": ""thankyou"", ""value"": """" },
				{ ""key"": ""end"", ""value"": """" },
				{ ""key"": ""bye"", ""value"": "" "" },
				{ ""key"": ""pluto"", ""value"": ""PLUTO"" },
				{ ""key"": ""earth"", ""value"": ""LA TIERRA"" },
				{ ""key"": ""milkyway"", ""value"": ""VÍA LÁCTEA"" },
				{ ""key"": ""visible"", ""value"": ""VISIBLE"" },
				{ ""key"": ""xray"", ""value"": ""RAYOS X"" },
				{ ""key"": ""neutrino"", ""value"": ""NEUTRINO"" },
				{ ""key"": ""blackhole"", ""value"": ""AGUJERO NEGRO"" },
				{ ""key"": ""sun"", ""value"": ""EL SOL"" },
				{ ""key"": ""map"", ""value"": ""Sistema Solar"" },
				{ ""key"": ""map01"", ""value"": ""Grupo Local"" },
				{ ""key"": ""alert"", ""value"": ""ERROR"" },
				{ ""key"": ""Ice"", ""value"": ""esIce"" },
				{ ""key"": ""Voyager"", ""value"": ""esVoyager"" },
				{ ""key"": ""Nothing"", ""value"": ""esNothing"" },
				{ ""key"": ""Extreme"", ""value"": ""esExtreme"" },
				{ ""key"": ""Credits"", ""value"": ""esCredits"" },
				{ ""key"": ""Earth"", ""value"": ""esEarth"" },
                { ""key"": ""start"", ""value"": ""INICIAR"" },
                { ""key"": ""language"", ""value"": ""IDIOMA"" },
                { ""key"": ""c0"", ""value"": "" "" },
				{ ""key"": ""c1"", ""value"": "" "" },
				{ ""key"": ""c2"", ""value"": ""Explorando el universo desde la Antártida"" },
				{ ""key"": ""c3"", ""value"": "" "" },
				{ ""key"": ""c4"", ""value"": ""Apoyado porNational Science Foundation Office of Polar Programs"" },
				{ ""key"": ""c5"", ""value"": ""Award #1612504: Exploring the Universe from Antarctica"" },
				{ ""key"": ""c6"", ""value"": ""Producido por FIELD DAY"" },
				{ ""key"": ""c7"", ""value"": ""en collaboración con"" },
				{ ""key"": ""c8"", ""value"": ""Wisconsin Institute for Discovery Virtual Environments group, "" }, 
				{ ""key"": ""c9"", ""value"": ""y Wisconsin IceCube Particle Astrophysics Center (WIPAC)"" },
				{ ""key"": ""c10"", ""value"": ""PI: Kevin Ponto"" },
				{ ""key"": ""c11"", ""value"": ""Productor: David Gagnon"" },
				{ ""key"": ""c12"", ""value"": ""Ingeniero: Phil Dougherty"" },
				{ ""key"": ""c13"", ""value"": ""Desarolladores: Katherine Ceballos, Simon Smith, Ross Tredinnick"" },
				{ ""key"": ""c14"", ""value"": ""Dirección Artística: Sarah Gagnon"" },
				{ ""key"": ""c15"", ""value"": ""Arte / UX: Eric Lang"" },
				{ ""key"": ""c16"", ""value"": ""Modelado 3D: Eric Peterson"" },
				{ ""key"": ""c17"", ""value"": ""Sonido y Música: Andrew Fitzpatrik"" },
				{ ""key"": ""c18"", ""value"": ""Escritora: Lindy Biller"" },
				{ ""key"": ""c19"", ""value"": ""WIPAC - Silvia Bravo-Gallart, James Madsen"" },
				{ ""key"": ""c20"", ""value"": ""Un producto de UW-Madison y IceCube"" },
				{ ""key"": ""c21"", ""value"": """" }]}";	
			}
			else if(fileName.Equals("localizedText_pt.json"))
			{
					jsonString = @"{""items"": [
				{ ""key"": ""antarctica"", ""value"": ""Ei! Venha cá!"" },
				{ ""key"": ""antarctica01"", ""value"": ""... Olá? ... Me desculpe-"" },
				{ ""key"": ""antarctica02"", ""value"": ""Ainda estamos pegando o jeito desse novo traje."" },
				{ ""key"": ""antarctica03"", ""value"": ""Me avise se estiver funcionando-"" },
				{ ""key"": ""antarctica04"", ""value"": ""Estou iniciando o display de realidade aumentada no seu headset agora..."" },
				{ ""key"": ""antarctica05"", ""value"": ""Ok. Você poderia olhar para cima, em direção ao ponto focal, para mim?"" },
				{ ""key"": ""antarctica06"", ""value"": ""Ótimo! Agora olhe para o outro ponto em direção ao seu pé."" },
				{ ""key"": ""antarctica07"", ""value"": ""Beleza! Tudo parece estar em ordem."" },
				{ ""key"": ""antarctica08"", ""value"": ""Seja bem-vindo ao IceCube!"" },
				{ ""key"": ""antarctica09"", ""value"": ""Estou feliz de ver que conseguiu percorrer todo o caminho"" },
				{ ""key"": ""antarctica10"", ""value"": ""até a Antártica para esta missão."" },
				{ ""key"": ""antarctica11"", ""value"": ""Antes de enviá-lo para a missão,"" },
				{ ""key"": ""antarctica12"", ""value"": ""vamos informá-lo sobre por que você está aqui:"" },
				{ ""key"": ""antarctica13"", ""value"": ""O centro de pesquisa IceCube detecta partículas chamadas neutrinos"" },
				{ ""key"": ""antarctica14"", ""value"": ""enviadas do espaço sideral."" },
				{ ""key"": ""antarctica15"", ""value"": ""Vou lhe mostrar os sensores em seu capacete."" },
				{ ""key"": ""antarctica16"", ""value"": ""Consegue ver aquela grade abaixo do centro de pesquisa?"" },
				{ ""key"": ""antarctica17"", ""value"": ""Cada ponto naquela grade é um sensor que detecta luz"" },
				{ ""key"": ""antarctica18"", ""value"": ""de neutrinos que passam por ele."" },
				{ ""key"": ""antarctica19"", ""value"": ""Olhe! Um neutrino acabou de ser detectado!"" },
				{ ""key"": ""antarctica20"", ""value"": ""O momento foi perfeito-"" },
				{ ""key"": ""antarctica21"", ""value"": ""Usaremos a informação do sensor"" },
				{ ""key"": ""antarctica22"", ""value"": ""para localizar de que parte do espaço sideral o neutrino veio..."" },
				{ ""key"": ""antarctica23"", ""value"": ""Eeeeee... pronto!"" },
				{ ""key"": ""antarctica24"", ""value"": ""Agora o seu trabalho é seguir esta trajetória"" },
				{ ""key"": ""antarctica25"", ""value"": ""pelo espaço sideral e encontrar a origem do neutrino detectado."" },
				{ ""key"": ""antarctica26"", ""value"": ""Você usará a habilidade \""Movimento Impossível\"" do seu traje"" },
				{ ""key"": ""antarctica27"", ""value"": ""e o caminho do neutrino."" },
				{ ""key"": ""antarctica28"", ""value"": ""Tudo o que você terá que fazer"" },
				{ ""key"": ""antarctica29"", ""value"": ""é olhar para o ponto focal no final do caminho... Tente agora."" },
				{ ""key"": ""voyager"", ""value"": ""Olá? ..."" },
				{ ""key"": ""voyager01"", ""value"": ""Você ainda está aí?"" },
				{ ""key"": ""voyager02"", ""value"": ""Você chegou inteirinho?"" },
				{ ""key"": ""voyager03"", ""value"": ""Pause um pouco para olhar ao redor e se situar-"" },
				{ ""key"": ""voyager04"", ""value"": ""é muito legal"" },
				{ ""key"": ""voyager05"", ""value"": ""estar mais longe no espaço sideral que qualquer outro humano já esteve!"" },
				{ ""key"": ""voyager06"", ""value"": ""Agora você tem um dever a ser feito."" },
				{ ""key"": ""voyager07"", ""value"": ""Siga o caminho do neutrino"" },
				{ ""key"": ""voyager08"", ""value"": ""que foi detectado pelo IceCube"" },
				{ ""key"": ""voyager09"", ""value"": ""para descobrir a origem da partícula."" },
				{ ""key"": ""voyager10"", ""value"": ""Enquanto nós esperamos o \""Movimento Impossível\"" recarregar,"" },
				{ ""key"": ""voyager11"", ""value"": ""vamos explicar algumas das outras características do seu traje."" },
				{ ""key"": ""voyager12"", ""value"": ""Se você olhar para os seus pés,"" },
				{ ""key"": ""voyager13"", ""value"": ""você pode usar os pontos focais"" },
				{ ""key"": ""voyager14"", ""value"": ""para mudar a visão do seu capacete."" },
				{ ""key"": ""voyager15"", ""value"": ""Tente-"" },
				{ ""key"": ""voyager16"", ""value"": ""olhe para os seus pés e mude para a visão de raios-X."" },
				{ ""key"": ""voyager17"", ""value"": ""Muito legal, né?"" },
				{ ""key"": ""voyager18"", ""value"": ""Olhe ao redor- Veja a galáxia!"" },
				{ ""key"": ""voyager19"", ""value"": ""Isso é como o universo parece quando observado por raios-X."" },
				{ ""key"": ""voyager20"", ""value"": ""Seu capacete está detectando raios-X"" },
				{ ""key"": ""voyager21"", ""value"": ""do mesmo modo que seus olhos normalmente enxergão a luz."" },
				{ ""key"": ""voyager22"", ""value"": ""Você poderia olhar para Plutão por alguns segundos?"" },
				{ ""key"": ""voyager23"", ""value"": ""Consegue ver como Plutão é só uma grande bola preta?"" },
				{ ""key"": ""voyager24"", ""value"": ""Isso é por que os raios-X não passam por ele."" },
				{ ""key"": ""voyager25"", ""value"": ""Agora, vamos mudar para a visão de neutrino."" },
				{ ""key"": ""alright"", ""value"": ""Beleza- olhe novamente para Plutão:"" },
				{ ""key"": ""neutrino01"", ""value"": ""para onde ele foi?!"" },
				{ ""key"": ""neutrino02"", ""value"": ""Plutão parece ter desaparecido!"" },
				{ ""key"": ""neutrino03"", ""value"": ""Seu capacete está agora só detectando neutrinos."" },
				{ ""key"": ""neutrino04"", ""value"": ""Os Neutrinos atravessam  praticamente tudo,"" },
				{ ""key"": ""neutrino05"", ""value"": ""Até planetas inteiros!"" },
				{ ""key"": ""neutrino06"", ""value"": ""É como se Plutão nem mesmo existisse para eles!"" },
				{ ""key"": ""neutrino07"", ""value"": ""Quando estiver pronto,"" },
				{ ""key"": ""neutrino08"", ""value"": ""Olhe para o ponto focal no final do caminho do neutrino-"" },
				{ ""key"": ""neutrino09"", ""value"": ""e você seguirá a partícula que foi detectada pelo IceCube."" },
				{ ""key"": ""journey"", ""value"": ""Estamos recebendo algumas... leituras bastante intensas."" },
				{ ""key"": ""journey01"", ""value"": ""Você está... muito longe no espaço sideral."" },
				{ ""key"": ""journey02"", ""value"": ""Ok- hora de passar-detalhes de sua missão."" },
				{ ""key"": ""journey03"", ""value"": ""Como você acabou de ver, nós demos ao seu traje a habilidade"" },
				{ ""key"": ""journey04"", ""value"": ""de ver em três diferentes modos:"" },
				{ ""key"": ""journey05"", ""value"": ""Luz visível, visão de raio-X, e detecção de neutrino."" },
				{ ""key"": ""journey06"", ""value"": ""Os dois primeiros vêm sendo usados por décadas para observar o espaço."" },
				{ ""key"": ""journey07"", ""value"": ""Mas se quisermos ver muito longe,"" },
				{ ""key"": ""journey08"", ""value"": ""Neutrinos são a única forma conhecida de se fazer isso."" },
				{ ""key"": ""journey09"", ""value"": ""É por isso que precisamos do observatório IceCube."" },
				{ ""key"": ""journey10"", ""value"": ""As linhas de sensores na Antártica"" },
				{ ""key"": ""journey11"", ""value"": ""nos deixam detectar neutrinos vindo do espaço sideral."" },
				{ ""key"": ""journey12"", ""value"": ""Isso nos ajuda a mapear partes do universo"" },
				{ ""key"": ""journey13"", ""value"": ""invisíveis aos telescópios."" },
				{ ""key"": ""journey14"", ""value"": ""A pergunta que você tem de responder é:"" },
				{ ""key"": ""journey15"", ""value"": ""O que foi que enviou o neutrino"" },
				{ ""key"": ""journey16"", ""value"": ""detectado pela IceCube aqui na Terra?"" },
				{ ""key"": ""journey17"", ""value"": ""Quando você encontrar a fonte no fim da sua jornada,"" },
				{ ""key"": ""journey18"", ""value"": ""você terá de coletar informações dela"" },
				{ ""key"": ""journey19"", ""value"": ""usando cada um dos três métodos que lhe demos."" },
				{ ""key"": ""journey20"", ""value"": ""Você usará suas visões de luz visível,"" },
				{ ""key"": ""journey21"", ""value"": ""de raios-X, e de detecção de neutrino-"" },
				{ ""key"": ""journey22"", ""value"": ""para coletar essas iinformações.."" },
				{ ""key"": ""journey23"", ""value"": ""Ok. As coisas podem ser arriscadas daqui para frente."" },
				{ ""key"": ""journey24"", ""value"": ""Boa sorte."" },
				{ ""key"": ""hi"", ""value"": ""Olá?"" },
				{ ""key"": ""ask"", ""value"": ""Está naà escuta?"" },
				{ ""key"": ""discover"", ""value"": ""Você descobriu um buraco negro!"" },
				{ ""key"": ""scan"", ""value"": ""Você tem que escaneá-lo com cada um"" },
				{ ""key"": ""scan01"", ""value"": ""de seus módulos de visão, rápido!"" },
				{ ""key"": ""getout"", ""value"": ""Depois SAIA daí!"" },
				{ ""key"": ""light"", ""value"": ""Certifique-se de que selecionou a visão de luz visível-"" },
				{ ""key"": ""collect"", ""value"": ""olhe para o buraco negro e colete dados de luz visível!"" },
				{ ""key"": ""collect01"", ""value"": ""Olhe logo acima do buraco negro, e colete os dados de raios-X!"" },
				{ ""key"": ""collect02"", ""value"": ""Olhe novamente para o buraco negro, e colete os dados de neutrino!"" },
				{ ""key"": ""nice"", ""value"": ""Você conseguiu! Nós temos as informações!"" },
				{ ""key"": ""path"", ""value"": ""Agora siga o caminho do neutrino de volta para a Terra! RÁPIDO!"" },
				{ ""key"": ""wow"", ""value"": ""Uau! Você conseguiu!"" },
				{ ""key"": ""wow01"", ""value"": ""Eu... não acredito que você está vivo!"" },
				{ ""key"": ""congrats"", ""value"": ""....quero dizer, hummm.. Parabéns, agente!"" },
				{ ""key"": ""source"", ""value"": ""Parece que a fonte da partícula de neutrino detectada pela IceCube"" },
				{ ""key"": ""source01"", ""value"": ""era um buraco negro!"" },
				{ ""key"": ""bh"", ""value"": ""Buracos negros são um dos mais estranhos,"" },
				{ ""key"": ""blackhole01"", ""value"": ""mais extremos objetos no universo todo!"" },
				{ ""key"": ""blackhole02"", ""value"": ""Você sabia que buracos negros podem ter a massa"" },
				{ ""key"": ""sn"", ""value"": ""de vários milhões de Sóis?"" },
				{ ""key"": ""blackhole03"", ""value"": ""Uma colher de buraco negro pode pesar"" },
				{ ""key"": ""blackhole04"", ""value"": ""tanto quanto um planeta inteiro!"" },
				{ ""key"": ""blackhole05"", ""value"": ""Eles também emitem neutrinos de alta energia"" },
				{ ""key"": ""blackhole06"", ""value"": ""que viajam milhões de anos-luz até a Terra."" },
				{ ""key"": ""blackhole07"", ""value"": ""Isso teria passado completamente despercebido"" },
				{ ""key"": ""blackhole08"", ""value"": ""se não fosse pelos cientistas do IceCube."" },
				{ ""key"": ""blackhole09"", ""value"": ""Buracos negros são muito difíceis de ser detectados-"" },
				{ ""key"": ""blackhole10"", ""value"": ""bem- porque eles são pretos!"" },
				{ ""key"": ""blackhole11"", ""value"": ""É impossível de ver algo preto na escuridão do espaço sideral!"" },
				{ ""key"": ""thankyou"", ""value"": ""Felizmente,""},
				{ ""key"": ""thankyou02"", ""value"": ""o IceCube encontrou um método de observá-los usando neutrinos!"" },
				{ ""key"": ""end"", ""value"": ""Bem, de nossa partes, essat missão está completa!"" },
				{ ""key"": ""bye"", ""value"": ""Até a próxima!"" },
				{ ""key"": ""pluto"", ""value"": ""PLUTÃO"" },
				{ ""key"": ""earth"", ""value"": ""TERRA"" },
				{ ""key"": ""milkyway"", ""value"": ""VIA LÁCTEA"" },
				{ ""key"": ""visible"", ""value"": ""VISÍVEL"" },
				{ ""key"": ""xray"", ""value"": ""RAIOS-X"" },
				{ ""key"": ""neutrino"", ""value"": ""NEUTRINO"" },
				{ ""key"": ""blackhole"", ""value"": ""BURACO NEGRO"" },
				{ ""key"": ""sun"", ""value"": ""SOL"" },
				{ ""key"": ""map"", ""value"": ""Sistema Solar"" },
				{ ""key"": ""map01"", ""value"": ""Grupo Local"" },
				{ ""key"": ""alert"", ""value"": ""ALERTA"" },
				{ ""key"": ""Ice"", ""value"": ""Gelo"" },
				{ ""key"": ""Voyager"", ""value"": ""Viajante"" },
				{ ""key"": ""Nothing"", ""value"": ""Nada"" },
				{ ""key"": ""Extreme"", ""value"": ""Extremo"" },
				{ ""key"": ""Credits"", ""value"": ""Créditos"" },
				{ ""key"": ""Earth"", ""value"": ""Terra"" },
                { ""key"": ""start"", ""value"": ""INICIAR"" },
                { ""key"": ""language"", ""value"": ""LÍNGUA"" },
				{ ""key"": ""c0"", ""value"": "" "" },
				{ ""key"": ""c1"", ""value"": "" "" },
				{ ""key"": ""c2"", ""value"": ""Explorando o Universo da Antártida"" },
				{ ""key"": ""c3"", ""value"": "" "" },
				{ ""key"": ""c4"", ""value"": ""Supported by the National Science Foundation Office of Polar Programs"" },
				{ ""key"": ""c5"", ""value"": ""Award #1612504: Exploring the Universe from Antarctica"" },
				{ ""key"": ""c6"", ""value"": ""Produzido por FIELD DAY"" },
				{ ""key"": ""c7"", ""value"": ""em colaboração com a Wisconsin Institute for Discovery Virtual Environments group"" },
				{ ""key"": ""c8"", ""value"": ""y Wisconsin IceCube Particle Astrophysics Center (WIPAC)"" },
				{ ""key"": ""c9"", ""value"": ""PI: Kevin Ponto"" },
				{ ""key"": ""c10"", ""value"": ""Desenvolvimento: Katherine Ceballos, Simon Smith, Ross Tredinnick"" },
				{ ""key"": ""c11"", ""value"": ""Produtor: David Gagnon"" },
				{ ""key"": ""c12"", ""value"": ""Engenheiro: Phil Dougherty"" },
				{ ""key"": ""c13"", ""value"": ""Direção de arte: Sarah Gagnon"" },
				{ ""key"": ""c14"", ""value"": ""Arte / UX: Eric Lang"" },
				{ ""key"": ""c15"", ""value"": ""Modelagem 3D: Eric Peterson"" },
				{ ""key"": ""c16"", ""value"": ""Música e Som: Andrew Fitzpatrik"" },
				{ ""key"": ""c17"", ""value"": ""Escrita: Lindy Biller"" },
				{ ""key"": ""c18"", ""value"": ""WIPAC - Silvia Bravo Gallary, James Madsen"" },
				{ ""key"": ""c19"", ""value"": ""Um produto da UW-Madison & IceCube"" },
				{ ""key"": ""c20"", ""value"": ""Financiado pela National Science Foundation"" },
				{ ""key"": ""c21"", ""value"": """" }]}";				
			}
			//Debug.Log(Application.streamingAssetsPath + "/" + fileName);
			//Debug.Log(reader.text);
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(jsonString);

			for (int i = 0; i < loadedData.items.Length; i++)
			{
				localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
			}
            
        //}
        /*else
        {
            //localizedText = new Dictionary<string, string>(); // localizedText.Clear();
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);

                LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

                for (int i = 0; i < loadedData.items.Length; i++)
                {
                    localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
                }

                //Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
            }
            else
            {
                Debug.LogError("Cannot find file!");
            }
        }*/

        if (fileName.Equals("localizedText_en.json"))
        {
			english = true;
            spanish = false;
            portuguese = false;
        }
        if(fileName.Equals("localizedText_es.json"))
        {
			english = false;
            spanish = true;
			portuguese = false;
        }
        if(fileName.Equals("localizedText_pt.json"))
        {
			english = false;
			spanish = false;
            portuguese = true;
        }

        isReady = true;
        updateLocalizedTexts();
    }

    public void updateLocalizedTexts()
    {
        LocalizedText[] components = Object.FindObjectsOfType<LocalizedText>();
        foreach(LocalizedText component in components)
        {
            Text text = component.gameObject.GetComponent<Text>();
            text.text = LocalizationManager.instance.GetLocalizedValue(component.key);
        }
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
		localizedText.TryGetValue(key, out result);
        //if (localizedText.ContainsKey(key))
        //{
        //    result = localizedText[key];
        //}
        return result;
    }

    public bool GetIsReady()
    {
        return isReady;
    }

}