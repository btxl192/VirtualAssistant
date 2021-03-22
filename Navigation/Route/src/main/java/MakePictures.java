import java.io.File;
import java.io.IOException;
import java.util.concurrent.CountDownLatch;

public class MakePictures implements Runnable{

    private final String img;
    private final String file;
    private final int floor;
    private final String room;
    private CountDownLatch latch;

    public MakePictures(String img, String file, int floor, String room, CountDownLatch latch){
        this.img = img;
        this.file = file;
        this.floor = floor;
        this.room = room;
        this.latch = latch;
    }

    @Override
    public void run() {
//        System.out.println("Started floor: " + floor + ", room: " + room);
        Route route = new Route(img, file, floor, room);
        route.run();
//        System.out.println("Finished floor: " + floor + ", room: " + room);
        try {
            String picName = floor + "/" + room + "Pic.png";
            String mp4VidName = floor + "/" + room + "Vid.mp4";
            String webmVidName = floor + "/" + room + ".webm";
            Process p = Runtime.getRuntime().exec("./ffmpeg.exe -r 1/5 -i " + picName + " -c:v libx264 -r 1/5 -pix_fmt yuv420p " + mp4VidName);
            p.waitFor();
            p = Runtime.getRuntime().exec("./ffmpeg.exe -i " + mp4VidName + " -c:v libvpx -b:v 1M -c:a libvorbis " + webmVidName);
            p.waitFor();
            File file = new File(mp4VidName);
            file.delete();
            file = new File(picName);
            file.delete();
        } catch (InterruptedException | IOException e) {
            System.out.println("Failed to produce video for " + room + " on floor " + floor + " !");
        }
        latch.countDown();
    }
}
