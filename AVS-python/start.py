import time
import signal
from voice_engine.source import Source
from voice_engine.kws import KWS
from alexa import Alexa
import logging

logging.basicConfig(level=logging.WARNING)


src = Source(rate=16000)
kws = KWS(model='alexa', sensitivity=0.5)
alexa = Alexa()

src.pipeline(kws, alexa)

def on_detected(keyword):
    print('detected {}'.format(keyword))
    alexa.listen()

kws.set_callback(on_detected)

is_quit = []
def signal_handler(signal, frame):
    print('Quit')
    is_quit.append(True)

signal.signal(signal.SIGINT, signal_handler)

src.pipeline_start()
while not is_quit:
    time.sleep(1)
src.pipeline_stop()
