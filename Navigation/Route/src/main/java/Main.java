import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.CountDownLatch;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

public class Main {

    private static CountDownLatch latch;

    private static int getBlueFloor(){
        JSONParser jsonParser = new JSONParser();
        Object obj = null;
        try (FileReader reader = new FileReader("Blue.json"))
        {
            obj = jsonParser.parse(reader);
        } catch (Exception e) {
            e.printStackTrace();
        }
        JSONArray array = new JSONArray();
        array.add(obj);
        JSONObject da = new JSONObject((Map) array.get(0));
        return Integer.parseInt(String.valueOf(da.keySet().toArray()[0]));
    }

    private static ArrayList<Integer> floorNums(){
        ArrayList<Integer> result = new ArrayList<>();

        File f = new File(System.getProperty("user.dir"));
        String[] pathNames = f.list();
        for (String pathname : pathNames) {
            if (pathname.contains(".json") && pathname.contains("floor")) {
                String[] parts = pathname.split(".json");
                String num = parts[0].split("floor")[1];
                result.add(Integer.parseInt(num));
            }
        }
        return result;
    }

    private static Set<String> listFloorRooms (int floor) {
        JSONParser jsonParser = new JSONParser();
        JSONObject floorJson = null;
        try (FileReader reader = new FileReader("floor" + floor + ".json"))
        {
            Object obj = jsonParser.parse(reader);
            JSONArray array = new JSONArray();
            array.add(obj);
            JSONObject da = new JSONObject((Map) array.get(0));
            floorJson =  new JSONObject ((Map) da.get(Integer.toString(floor)));
        } catch (Exception e) {
            e.printStackTrace();
        }
        return floorJson.keySet();
    }

    public static void main(String[] args) {
        Process p = null;
        try {
            p = Runtime.getRuntime().exec("python gui.py");
            while(true){
                if(!p.isAlive()){
                    break;
                }
            }
        } catch (IOException e) {
            System.out.println("Failed to run the Python GUI!");
        }
        int totalRooms = 0;
        ArrayList<Integer> floors = floorNums();
        for (int floor : floors) {
            Set<String> rooms = listFloorRooms(floor);
            totalRooms += rooms.size() - 2;
        }
        int blueFloor = getBlueFloor();
        latch = new CountDownLatch(totalRooms);
        for (int floor : floors) {
            Set<String> rooms = listFloorRooms(floor);
            for (String room : rooms) {
                if (!room.equals("hall") && (!room.equals("lift") || (floor == blueFloor && room.equals("lift")))) {
                    String image = "floor" + floor + ".jpg";
                    String file = "floor" + floor + ".json";
                    Thread thread = new Thread(new MakePictures(image, file, floor, room, latch));
                    thread.start();
                }
            }
        }
        try {
            latch.await();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
        File f = new File(System.getProperty("user.dir"));
        String[] pathNames = f.list();
        for (String pathname : pathNames) {
            if (pathname.contains(".json") && !pathname.equals("Blue.json")) {
                File file = new File(pathname);
                file.delete();
            }
            if (pathname.contains(".jpg") && pathname.contains("floor")) {
                File file = new File(pathname);
                file.delete();
            }
        }
    }
}