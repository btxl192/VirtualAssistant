import java.util.ArrayList;

public class LinkedList {

    private Element first;
    private int size;

    private class Element {

        private int x;
        private int y;
        private double dist;
        private int length;
        private ArrayList<ArrayList<Integer>> route;
        private Element next;

        public Element(int x, int y, double dist, int length){
            this.x = x;
            this.y = y;
            this.dist = dist;
            this.length = length;
            this.route = new ArrayList<>();
            this.next = null;
        }

        public int getX(){
            return this.x;
        }

        public int getY(){
            return this.y;
        }

        public double getDist(){
            return this.dist;
        }

        public int getLength(){
            return this.length;
        }

        public ArrayList<ArrayList<Integer>> getPrev(){
            return this.route;
        }

        public Element getNext(){
            return this.next;
        }

        public void setNext(Element el){
            this.next = el;
        }

        public double getValue(){
            return this.dist + this.length;
        }

        public void appendPrevious(ArrayList<ArrayList<Integer>> route){
            this.route = new ArrayList<>(route);
        }

    }

    public LinkedList(){
        this.first = null;
        this.size = 0;
    }

    public void addElement(int x, int y, double dist, int length, ArrayList<ArrayList<Integer>> prev) {
        Element el = new Element(x, y, dist, length);
        el.appendPrevious(prev);
        if (this.first == null){
            this.first = el;
            this.size++;
            return;
        }
        if (this.first.getValue() > el.getValue()) {
            Element dummy = this.first;
            el.setNext(dummy);
            this.first = el;
            this.size++;
            return;
        }
        Element current = this.first;
        for (int i = 0; i < this.size; i++){
            if (current.getNext() == null){
                current.setNext(el);
                break;
            }
            if (current.getNext().getValue() > el.getValue()){
                Element dummy = current.getNext();
                el.setNext(dummy);
                current.setNext(el);
                break;
            }
            current = current.getNext();
        }
        this.size++;
    }

    public ArrayList<Object> getElement(){
        Element dummy = this.first;
        ArrayList<Object> result = new ArrayList<>();
        result.add(dummy.getX());
        result.add(dummy.getY());
        result.add(dummy.getDist());
        result.add(dummy.getLength());
        result.add(dummy.getPrev());
        this.first = this.first.getNext();
        this.size--;
        return result;
    }

    public boolean isEmpty(){
        if (this.size <= 0){
            return true;
        }
        return false;
    }

    public void printElements(Element a){
        if (a == null) {
            return;
        }
        System.out.print(a.getX());
        System.out.print(" ");
        System.out.print(a.getY());
        System.out.print(" ");
        System.out.print(a.getDist());
        System.out.print(" ");
        System.out.print(a.getLength());
        System.out.print(" ");
        System.out.print(a.getPrev());
        System.out.print("\n");
        printElements(a.getNext());
    }

    public void print(){
        printElements(this.first);
    }

}

//import java.util.ArrayList;
//        import java.awt.image.BufferedImage;
//        import java.io.IOException;
//        import javax.imageio.ImageIO;;
//        import java.io.File;
//
//public class Route {
//
//    // public static void main(String args[]) throws Exception{
//    // 	BufferedImage bi = ImageIO.read(new File("newPlanResolution.jpg"));
//    // 	int[] rgbData = bi.getRGB(0,0, bi.getWidth(), bi.getHeight(),
//    // 	        null, 0,bi.getWidth());
//    // 	int colorRed=(rgbData[(55*bi.getWidth())+431] >> 16) & 0xFF;
//
//    // 	// colorGreen=(rgbData[(y*bi.getWidth())+x] >> 8) & 0xFF;
//
//    // 	// colorBlue=(rgbData[(y*bi.getWidth())+x]) & 0xFF;
//    // 	System.out.println(colorRed);
//    // }
