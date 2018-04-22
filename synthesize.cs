using System.Runtime.Serialization;
using System;
using System.Globalization;

namespace SynthesizeVoice
{
    [DataContract(Name="inputConfig")]
    public class SynthesisInput
    {
        [DataMember(Name="text")]
        public string Text { get; set; }
        
        [DataMember(Name="ssml")]
        public string Ssml { get; set; }
    }

    [DataContract(Name="voiceConfig")]
    public class VoiceSelectionParams
    {
        [DataMember(Name="languageCode")]
        public string LanguageCode { get; set; }
        
        [DataMember(Name="name")]
        public string Name { get; set; }
        
        [DataMember(Name="ssmlGender")]
        public string Gender { get; set; }
    }


    [DataContract(Name="audioConfig")]
    public class AudioConfig
    {
        [DataMember(Name="audioEncoding")]
        public string AudioEncoding { get; set; }
        
        [DataMember(Name="pitch")]
        public float Pitch { get; set; }
        
        [DataMember(Name="speakingRate")]
        public float SpeakingRate { get; set; }
        
        [DataMember(Name="volumeGainDb")]
        public float VolumeGainDb { get; set; }
    }

    [DataContract(Name="synthesizeRequest")]
    public class SynthesizeRequest
    {
        [DataMember(Name="input")]
        public SynthesisInput InputConfig { get; set; }
        
        [DataMember(Name="voice")]
        public VoiceSelectionParams VoiceConfig { get; set; }
        
        [DataMember(Name="audioConfig")]
        public AudioConfig AudioConfig { get; set; }

        public static SynthesizeRequest CreateMinimalRequest(string text) => new SynthesizeRequest()
        {
            InputConfig = new SynthesisInput()
            {
                Text = text
            },
            VoiceConfig = new VoiceSelectionParams()
            {
                LanguageCode = "en-US",
                Name = "en-US-Wavenet-D"
            },
            AudioConfig = new AudioConfig()
            {
                AudioEncoding = "LINEAR16"
            }
        };
    }

    [DataContract(Name="synthesizeResponse")]
    public class SynthesizeResponse
    {
        [DataMember(Name="audioContent")]
        public string AudioContent { get; set; }
    }
}