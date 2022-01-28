import wrapper
import time

speech = wrapper.Speech

speech.Initialize(self=speech)

speech.Say(self=speech, text="This is a very very very long string.", interrupt=False)

time.sleep(1)

speech.Say(self=speech, text="I interrupted :)", interrupt=True)

speech.Close(self=speech)