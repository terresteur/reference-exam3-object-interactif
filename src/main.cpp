#include <Arduino.h>
#include <M5_Encoder.h>
#include <MicroOscSlip.h>

MicroOscSlip<128> monOsc(& Serial);

M5_Encoder myEncoder;
unsigned long monChronoDepart ;

void setup() {

  Serial.begin(115200);

  Wire.begin();
  myEncoder.begin();
}

void loop() {


 if ( millis() - monChronoDepart >= 20 ) { 
  monChronoDepart = millis(); 
  int valeurEncodeur = myEncoder.getEncoderRotation();
  int changementEncodeur = myEncoder.getEncoderChange();
  int etatBouton = myEncoder.getButtonState();
  //Serial.println(valeurEncodeur);
 
  if(changementEncodeur < 0) {
   myEncoder.setLEDColorLeft( 0, 255, 0 );
   myEncoder.setLEDColorRight( 0, 0, 0 );
  } else if(changementEncodeur > 0) {
   myEncoder.setLEDColorRight( 0, 255, 0 );
   myEncoder.setLEDColorLeft( 0, 0, 0 );
  }

  monOsc.sendInt("/angle", changementEncodeur);
  monOsc.sendInt("/bouton", etatBouton);
  myEncoder.update();

}


}

