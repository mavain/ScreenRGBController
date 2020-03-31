#define RED_PIN 7
#define GREEN_PIN 6
#define BLUE_PIN 5

void setup() {

  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);

  analogWrite(RED_PIN, 255);
  analogWrite(GREEN_PIN, 255);
  analogWrite(BLUE_PIN, 255);

  Serial.begin(115200);

}

byte rgbData[11];

void readRGBData() {

  char beginningTag = '<';
  char endingTag = '>';

  int index = 0;
  char current = Serial.read();
  bool beginRead = false;

  while (current != beginningTag) {
    if (Serial.available() > 0) {
      current = Serial.read();
    }
  }

  while (beginRead && current == beginningTag) {
    if (Serial.available() > 0) {
      current = Serial.read();
    }
  }

  while (current != endingTag) {
    if (Serial.available() > 0) {
      rgbData[index++] = current;
      current = Serial.read();
    }
  }
  

}


byte red = 0;
byte green = 0;
byte blue = 0;

void loop() {

  readRGBData();

  red = (rgbData[0] - '0') * 100 + (rgbData[1] - '0') * 10 + (rgbData[2] - '0');
  green = (rgbData[4] - '0') * 100 + (rgbData[5] - '0') * 10 + (rgbData[6] - '0');
  blue = (rgbData[8] - '0') * 100 + (rgbData[9] - '0') * 10 + (rgbData[10] - '0');

  analogWrite(RED_PIN, red);
  analogWrite(GREEN_PIN, green);
  analogWrite(BLUE_PIN, blue);

}
