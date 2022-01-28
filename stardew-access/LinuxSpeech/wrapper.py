import speechd

class Speech:
    client = None

    def Initialize(self):
        self.client = speechd.SSIPClient('stardew-access')

    def Say(self, text, interrupt):
        if(self.client is not None):
            if(interrupt):
                self.client.stop()
            
            self.client.speak(text)
    
    def Close(self):
        if(self.client is not None):
            self.client.close()