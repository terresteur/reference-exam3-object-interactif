#include <Arduino.h>
#include <M5_PbHub.h>
#include <MicroOscSlip.h>

MicroOscSlip<128> monOsc(& Serial);

#define CANAL_KEY_UNIT_BOUTON 1
#define CANAL_KEY_UNIT_ANGLE 2  

unsigned long monChronoDepart ; //Utilisée pour savoir quand 20 ms sont passées dans la loop.

M5_PbHub myPbHub; //Permet d’utiliser toutes les fonctions du hub → digitalRead, analogRead, pixels…

void setup() {

  Serial.begin(115200); //communication série (nécessaire pour OSC).

  Wire.begin(); //initialise I2C, obligatoire pour le PbHub.
  myPbHub.begin(); //démarre le PbHub

  myPbHub.setPixelCount( CANAL_KEY_UNIT_BOUTON , 1); //dit combien de pixels LED tu veux sur ce canal.

}

//chaque fois qu’un message OSC arrive, cette fonction est appelée et reçoit le message à traiter.
void myOscMessageParser(MicroOscMessage & receivedOscMessage) {
  // Ici, un if et receivedOscMessage.checkOscAddress() est utilisé pour traiter les différents messages
  if (receivedOscMessage.checkOscAddress("/pixel")) {  // MODIFIER /pixel pour l'adresse qui sera reçue et Si c’est bien "/pixel", on entre dans ce bloc if
       int premierArgument = receivedOscMessage.nextAsInt(); // Récupérer le premier argument du message en tant que int
       int deuxiemerArgument = receivedOscMessage.nextAsInt(); // SI NÉCESSAIRE, récupérer un autre int
       int troisiemerArgument = receivedOscMessage.nextAsInt(); // SI NÉCESSAIRE, récupérer un autre int
       myPbHub.setPixelColor( CANAL_KEY_UNIT_BOUTON , 0 , premierArgument,deuxiemerArgument,troisiemerArgument ); // 0 → numéro de la LED dans ce canal (souvent 0 si tu n’as qu’une LED).
       // UTILISER ici les arguments récupérés
 
   // SI NÉCESSAIRE, ajouter d'autres if pour recevoir des messages avec d'autres adresses
   } else if (receivedOscMessage.checkOscAddress("/autre")) {  // MODIFIER /autre une autre adresse qui sera reçue
       // ...
   }
}

void loop() {

  monOsc.onOscMessageReceived(myOscMessageParser);

  if ( millis() - monChronoDepart >= 20 ) { 
    monChronoDepart = millis(); 

    int maLectureKey = myPbHub.digitalRead( CANAL_KEY_UNIT_BOUTON );
    monOsc.sendInt("/bouton", maLectureKey);

    int maLectureAngle = myPbHub.analogRead( CANAL_KEY_UNIT_ANGLE );
    //int rotation = maLectureAngle * 360 / 4095;
    monOsc.sendInt("/angle", maLectureAngle);
}

}

