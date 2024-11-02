namespace template_lambda;
public class ServiceEntry {
    public ServiceEntry(string musicList, string serviceTime) {
        MusicList = musicList;
        ServiceTime = serviceTime;
    }

    public string MusicList { get; set; }
    public string ServiceTime { get; set; }
}