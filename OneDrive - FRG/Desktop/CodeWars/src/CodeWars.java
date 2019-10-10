import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class CodeWars {
	
	//my solution
	public static int sortDesc(final int num) {
		String number = num + "";
		String returnString = "";
		String manip = "";
		if(number.length() == 1) return num;
		for(int i = 0; i < number.length(); i++)
			manip += " " + number.charAt(i);
		List<String> list  = Arrays.asList(manip.trim().split(" "));
		Collections.sort(list);
		Collections.reverse(list);
		for (String string : list) {
			returnString += string;
		}
		return Integer.parseInt(returnString);
	}
	//Better Version
	public static int sortDescCorrect(final int num) {
		return  Integer.parseInt(String.valueOf(num).chars()
				.mapToObj(i -> String.valueOf(Character.getNumericValue(i))).sorted(Comparator.reverseOrder())
				.collect(Collectors.joining()));
	}
	
	//my second test solution
	public static boolean isIsogram(String str) {
		Set<Character> characters = new TreeSet<>(); 
		for(int i = 0; i < str.length(); i++)
			characters.add(str.toLowerCase().charAt(i));
		return (characters.size() == str.length()) ?true : false;
		
		
	}
	//Better second test solution
	public static boolean  isIsogramCorrect(String str) {
	    return str.length() == str.toLowerCase().chars().distinct().count();
	} 
	
	 public static String createPhoneNumber(int[] numbers) {
		 String phoneNumber = "(";
	     for(int number : numbers){
	         phoneNumber +=(phoneNumber.length() == 4) ? ") " +number :(phoneNumber.length() == 9)? "-" + number: ( number+ "");
	      }
	      return phoneNumber; 
	 }  
	
	 public static String createPhoneNumberBest(int[] numbers) {
		    return String.format("(%d%d%d) %d%d%d-%d%d%d%d", java.util.stream.IntStream.of(numbers).boxed().toArray());
	}
	 
	 public static String makeReadable(int seconds) {
		    
		    return String.format("%dd%:%dd%:%dd%", ((seconds/60)/60), ((seconds/60)%60), (seconds%60));
	}
	public static void main(String[] args) {
		//System.out.println(sortDescCorrect(45321));
		//Test 2
		int[] phone = {1,2,3,4,5,6,7,8,9,0};
		System.out.println(makeReadable(120));
	}
}
	
