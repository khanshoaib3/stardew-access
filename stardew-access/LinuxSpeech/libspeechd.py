from multiprocessing.connection import wait
from threading import Thread
from time import time
import speechd
import time

client = speechd.SSIPClient('test')
client.speak("Hello World! this is yusuf")
time.sleep(1)
client.stop()
client.speak("No this is shoaib")
client.close()