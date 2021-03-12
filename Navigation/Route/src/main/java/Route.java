import java.awt.*;
import java.util.ArrayList;
import java.awt.image.BufferedImage;
import java.io.IOException;
import javax.imageio.ImageIO;;
import java.io.File;
import java.io.FileReader;
import java.util.Collections;
import java.util.Map;
import java.util.Set;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

public class Route {

    private int[] im;
    private LinkedList list;
    private ArrayList<ArrayList<Integer>> used;
    private int floor;
    private JSONObject file;
    private BufferedImage bi;
    private String imageName;
    private String room;

    public Route(String img, String file, int floor, String room){
        JSONParser jsonParser = new JSONParser();
        this.imageName = img;
        this.room = room;
        try {
            this.bi = ImageIO.read((new File(img)));
        } catch (IOException e) {
            System.out.println("Image " + img + " couldn't be open!");
        }
        this.im = this.bi.getRGB(0,0, this.bi.getWidth(), this.bi.getHeight(),
                null, 0,this.bi.getWidth());
        try (FileReader reader = new FileReader(file))
        {
            Object obj = jsonParser.parse(reader);
            JSONArray array = new JSONArray();
            array.add(obj);
            JSONObject da = new JSONObject((Map) array.get(0));
            this.file = new JSONObject ((Map) da.get(Integer.toString(floor)));
        } catch (Exception e) {
            e.printStackTrace();
        }
        this.floor = floor;
        this.used = new ArrayList<>();
        this.list = new LinkedList();
    }

    private int doorPixel(String room, String key){
        JSONObject r = new JSONObject ((Map) this.file.get(room));
        JSONArray rooms = (JSONArray) r.get(key);
        JSONObject h = new JSONObject ((Map) this.file.get("hall"));
        JSONArray hall = (JSONArray) h.get(key);
        for (int i = 0; i < rooms.size(); i++){
            JSONArray lis = (JSONArray) rooms.get(i);
            for (int j = 0; j < hall.size(); j++){
                JSONArray lists = (JSONArray) hall.get(j);
                int first = Integer.parseInt(String.valueOf(lis.get(0)));
                int second = Integer.parseInt(String.valueOf(lists.get(1)));
                if(Math.abs(first - second) < 20){
                    return second;
                }
                first = Integer.parseInt(String.valueOf(lists.get(0)));
                second = Integer.parseInt(String.valueOf(lis.get(1)));
                if(Math.abs(first - second) < 20){
                    return first;
                }
            }
        }
        return -1;
    }

    private int doorPixelParallel(String room, String keyR, String keyH){
        JSONObject r = new JSONObject ((Map) this.file.get(room));
        JSONArray rooms = (JSONArray) r.get(keyR);
        JSONArray lis = (JSONArray) rooms.get(rooms.size() - 1);
        JSONObject h = new JSONObject ((Map) this.file.get("hall"));
        JSONArray hall = (JSONArray) h.get(keyH);
        JSONArray lists = (JSONArray) rooms.get(hall.size() - 1);
        int first = -1;
        int last = -1;
        int checkR = Integer.parseInt(String.valueOf(lis.get(1)));
        int checkH = Integer.parseInt(String.valueOf(lists.get(1)));
        if (checkR < checkH){
            last = checkR;
        } else {
            last = checkH;
        }
        checkR = Integer.parseInt(String.valueOf(lis.get(0)));
        checkH = Integer.parseInt(String.valueOf(lists.get(0)));
        if(checkR > checkH){
            first = checkR;
        } else {
            first = checkH;
        }
        if (first != -1){
            return (first + last) / 2;
        }
        return -1;
    }

    private ArrayList<Integer> getEndPixel(String room){
        ArrayList<Integer> result = new ArrayList<>();
        JSONObject rooms = new JSONObject ((Map) this.file.get(room));
        JSONObject hall = new JSONObject ((Map) this.file.get("hall"));
        Set<String> keys_room = rooms.keySet();
        Set<String> keys_hall = hall.keySet();
        boolean flag = false;
        int first = -1;
        int last = -1;
        for (String key : keys_room){
            if (keys_hall.contains(key)){
                int x = doorPixel(room, key);
                if (x != -1) {
                    if (!flag){
                        first = Integer.parseInt(key);
                        flag = true;
                    }
                    last = Integer.parseInt(key);
                }
            }
        }
        int mid = (first + last) / 2;
        if (mid != -1) {
            result.add(doorPixel(room, String.valueOf(mid)));
            result.add(mid);
        }
        else{
            ArrayList<Integer> res = getEndPixelParallel(room);
            result.add(doorPixelParallel(room, String.valueOf(res.get(1)), String.valueOf(res.get(0))));
            result.add(res.get(0));
            return result;
        }
        return result;
    }

    private boolean checkRange(JSONArray range1, JSONArray range2){
        int x = Integer.parseInt(String.valueOf(range1.get(0)));
        int y = Integer.parseInt(String.valueOf(range1.get(1)));
        int c = Integer.parseInt(String.valueOf(range2.get(0)));
        int d = Integer.parseInt(String.valueOf(range2.get(1)));
        if (x <= c && y <=  d && y >= c){
            return true;
        }
        if (x >= c && x <= d && y >= d){
            return true;
        }
        if (x >= c && x <= d && y <= d && y >= c){
            return true;
        }
        if (x <= c && y >= d){
            return true;
        }
        return false;
    }

    private boolean checkNext(String room, String keyR, String keyH){
        JSONObject r = new JSONObject ((Map) this.file.get(room));
        JSONArray rooms = (JSONArray) r.get(keyR);
        JSONObject h = new JSONObject ((Map) this.file.get("hall"));
        JSONArray hall = (JSONArray) h.get(keyH);
        for (int i = 0; i < rooms.size(); i++){
            JSONArray m = (JSONArray) rooms.get(i);
            for (int j = 0; j < hall.size(); j++) {
                JSONArray n = (JSONArray) hall.get(j);
                if (checkRange(m, n)){
                    return true;
                }
            }
        }
        return false;
    }

    private ArrayList<Integer> getEndPixelParallel(String room){
        ArrayList<Integer> result = new ArrayList<>();
        JSONObject rooms = new JSONObject ((Map) this.file.get(room));
        JSONObject hall = new JSONObject ((Map) this.file.get("hall"));
        Set<String> keys_room = rooms.keySet();
        Set<String> keys_hall1 = hall.keySet();
        ArrayList<Integer> keys_hall = new ArrayList<>();
        for(String key : keys_hall1){
            keys_hall.add(Integer.parseInt(key));
        }
        Collections.sort(keys_hall);
        int last_room = -1;
        int first_room = 800;
        for (String key : keys_room){
            int i = Integer.parseInt(key);
            if (i > last_room){
                last_room = i;
            }
            if(i < first_room){
                first_room = i;
            }
        }
        for (int i : keys_hall){
            if (Math.abs(i - last_room) < 15){
                if (checkNext(room, String.valueOf(last_room), String.valueOf(i))){
                    result.add(i);
                    result.add(last_room);
                    return result;
                }
            }
        }
        for (int i = 0; i < keys_hall.size(); i++){
            int key = keys_hall.get(keys_hall.size() - 1 - i);
            if (Math.abs(key - first_room) < 15){
                if (checkNext(room, String.valueOf(first_room), String.valueOf(key))){
                    result.add(key);
                    result.add(first_room);
                }
            }
        }
        return result;
    }

    private double dist(int x, int y, int x1, int y1){
        double a = Math.pow(Math.abs(x - x1), 2);
        double b = Math.pow(Math.abs(y - y1), 2);
        return Math.sqrt(a+b);
    }

    private ArrayList<Integer> getStartPixels(){
        ArrayList<Integer> result = new ArrayList<>();
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
        int blueFloor = Integer.parseInt(String.valueOf(da.keySet().toArray()[0]));
        int x;
        int y;
        if (blueFloor == this.floor) {
            da = new JSONObject((Map) da.get(String.valueOf(blueFloor)));
            y = Integer.parseInt(String.valueOf(da.keySet().toArray()[0]));
            x = Integer.parseInt(String.valueOf(da.get(String.valueOf(y))));
            result.add(x);
            result.add(y);
        } else {
            result = getEndPixel("lift");
        }
        return result;
    }

    private boolean isBlack (int x, int y) {
        if (x >= this.im.length / 720){
            return true;
        }
        int red = (this.im[(y*bi.getWidth())+x] >> 16) & 0xFF;
        int green = (this.im[(y*bi.getWidth())+x] >> 8) & 0xFF;
        int blue = (this.im[(y*bi.getWidth())+x]) & 0xFF;
        if (red != green) {
            return false;
        }
        if (red != blue) {
            return false;
        }
        if (blue > 220) {
            return false;
        }
        return true;
    }

    private boolean isSomethingBlack (int x, int y, double dist, int len) {
        if (len < 8 || dist < 8) {
            return !isBlack(x, y);
        }
        for (int i = 0; i < 5; i++){
            if (x - i >= 0 && x - i < 1280) {
                for (int j = 0; j < 5; j++){
                    if (y - j >= 0 && y - j < 720) {
                        if (isBlack(x-i, y-j)){
                            return false;
                        }
                    }
                    if (y + j >= 0 && y + j < 720) {
                        if (isBlack(x-i, y+j)){
                            return false;
                        }
                    }
                }
            }
            if (x + i >= 0 && x + i < 1280) {
                for (int j = 0; j < 5; j++){
                    if (y - j >= 0 && y - j < 720) {
                        if (isBlack(x+i, y-j)){
                            return false;
                        }
                    }
                    if (y + j >= 0 && y + j < 720) {
                        if (isBlack(x+i, y+j)){
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private boolean checkPixel (int x, int y, double dist, int length) {
        if (x < 0 || x >= 1280) {
            return false;
        }
        if (y < 0 || y >= 720) {
            return false;
        }
        ArrayList<Integer> check = new ArrayList<>();
        check.add(x);
        check.add(y);
        if (this.used.contains(check)) {
            return  false;
        }
        return isSomethingBlack(x, y, dist, length);
    }

    private void addPixel (int x, int y, ArrayList<ArrayList<Integer>> prev, int length, int xs, int ys) {
        double dis = dist(xs, ys, x, y);
        if (checkPixel(x, y, dis, length)){
            this.list.addElement(x, y, dis, length, prev);
            ArrayList<Integer> dummy = new ArrayList<>();
            dummy.add(x);
            dummy.add(y);
            this.used.add(dummy);
        }
    }

    private ArrayList<ArrayList<Integer>> bestRoute (int xs, int ys) {
        ArrayList<Integer> start = getStartPixels();
        int startX = start.get(0);
        int startY = start.get(1);
        this.list.addElement(startX, startY, dist(startX, startY, xs, ys), 1, new ArrayList<>());
        ArrayList<Integer> dummy = new ArrayList<>();
        dummy.add(startX);
        dummy.add(startY);
        this.used.add(dummy);
        while (!this.list.isEmpty()) {
            ArrayList<Object> el = this.list.getElement();
            int x = (int) el.get(0);
            int y = (int) el.get(1);
            double dist = (double) el.get(2);
            int length = (int) el.get(3);
            ArrayList<ArrayList<Integer>> previous = (ArrayList<ArrayList<Integer>>) el.get(4);
            dummy = new ArrayList<>();
            dummy.add(x);
            dummy.add(y);
            previous.add(dummy);
            if (dist == 0) {
                return previous;
            }
            length ++;
            addPixel(x-1, y-1, previous, length, xs, ys);
            addPixel(x-1, y, previous, length, xs, ys);
            addPixel(x-1, y+1, previous, length, xs, ys);
            addPixel(x, y-1, previous, length, xs, ys);
            addPixel(x, y+1, previous, length, xs, ys);
            addPixel(x+1, y-1, previous, length, xs, ys);
            addPixel(x+1, y, previous, length, xs, ys);
            addPixel(x+1, y+1, previous, length, xs, ys);
        }
        return null;
    }

    private void drawPixel (int x, int y, BufferedImage img){
        if (!isBlack(x, y)){
            Color col = Color.red;
            img.setRGB(x, y, col.getRGB());
        }
    }

    private void drawRoute (int x, int y, BufferedImage img) {
        drawPixel(x-1, y-1, img);
        drawPixel(x-1, y, img);
        drawPixel(x-1, y+1, img);
        drawPixel(x, y-1, img);
        drawPixel(x, y, img);
        drawPixel(x, y+1, img);
        drawPixel(x+1, y-1, img);
        drawPixel(x+1, y, img);
        drawPixel(x+1, y+1, img);
    }

    private void drawRoom (String room, BufferedImage img) {
        JSONObject r = new JSONObject ((Map) this.file.get(room));
        Set<String> keys = r.keySet();
        for (String key : keys){
            JSONArray rooms = (JSONArray) r.get(key);
            for (int j = 0; j < rooms.size(); j++) {
                JSONArray m = (JSONArray) rooms.get(j);
                int first = Integer.parseInt(String.valueOf(m.get(0)));
                int second = Integer.parseInt(String.valueOf(m.get(1)));
                for (int i = first; i <= second; i++) {
                    drawPixel(i, Integer.parseInt(key), img);
                }
            }
        }
    }

    private void makePicture (String room, String name) throws IOException {
        BufferedImage img = ImageIO.read((new File(this.imageName)));
        ArrayList<Integer> end = getEndPixel(room);
        ArrayList<ArrayList<Integer>> route = bestRoute(end.get(0), end.get(1));
        if (route == null) {
            System.out.println("No route found");
            return;
        }
        for (ArrayList<Integer> pixel : route) {
            drawRoute(pixel.get(0), pixel.get(1), img);
        }
        drawRoom(room, img);
        File f = new File(name +".png");
        ImageIO.write(img, "PNG", f);
    }

    public void run() {
        try {
            makePicture(this.room, this.floor + "/" + this.room + "Pic");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
