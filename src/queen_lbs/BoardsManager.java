//Daniel Cohen 203850029 Ben Efrat 305773806

package queen_lbs;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.PriorityQueue;
import java.util.Scanner;

import queen_lbs.Queen.MyPair;



public class BoardsManager {

	PriorityQueue <Queen>  queue ;//priority queue for holding best score Boards

	private ArrayList<Queen> Kboard = null;
	private int X;
	private int K;
	private int N;
	private boolean isDone = false;
	private int iterations = 0;
	private int bestEverScore=Integer.MAX_VALUE;
	private ArrayList<MyPair[]> IndexesOfFirstBoards ;
	public class Queencomparator implements Comparator<Queen>{//override for comparing elements inside queue

		@Override
		public int compare(Queen x, Queen y) {
			if(x.score > y.score)
				return 1;
			else if(x.score<y.score)
				return -1;
			// TODO Auto-generated method stub
			return 0;
		}

	}


	public BoardsManager(int X, int K,int N)
	{ 
		queue = new PriorityQueue<Queen>(1,new Queencomparator()); 
		IndexesOfFirstBoards = new ArrayList<MyPair[]>();
		this.X = X;
		this.K = K;
		this.N = N;
		Kboard = new ArrayList<Queen>(N);
	}

	public void Solve() {
		System.out.println("iteration "+ iterations);
		generateFirstIterationBoards(); //create first iteration with k boards
		generateSonsIteration();
		while (iterations<X&&isDone == false )
		{
			Kboard = pickBestKs();//pick the best K Boards
			generateSonsIteration();
		}

		if(isDone == false)	
			System.out.println("Fail ! Couldnt Solve in "+ iterations+ " Iterations, Best Score Was "+bestEverScore);
	}


	public void generateFirstIterationBoards()
	{

		for(int i=0;i<K;i++)
		{
			boolean flag=true;
			Kboard.add(new Queen(N));
			while(flag == true)
			{
				Kboard.get(Kboard.size()-1).generateQueenBoard();
				 if(!(indexesEqual(Kboard.get(Kboard.size()-1).indexes)))//makes sure that the randomly generated Father Board isn't equal to another randomly father
				{
					flag = false;
					IndexesOfFirstBoards.add(Kboard.get(Kboard.size()-1).indexes);//add to list of Indexes the new Indexes of new unique Board
				}

			}
			System.out.println("First Steps - Board number "+(i+1));
			Kboard.get(Kboard.size()-1).CalcHioristic();
			Kboard.get(Kboard.size()-1).printBoard();
			if(Kboard.get(Kboard.size()-1).score==0) 
			{
				System.out.println(" finished after First K board creation "+ iterations + " iterations");
				Kboard.get(Kboard.size()-1).printBoard();
				isDone=true;
				break;
			}
		}
	}

	private boolean indexesEqual(MyPair[] indexes) {
		// TODO Auto-generated method stub
		for(int i=0;i<IndexesOfFirstBoards.size();i++)
		{
			if(isEqual(IndexesOfFirstBoards.get(i),indexes))
				return true;
		}

		return false;
	}
	public boolean isEqual(MyPair[] Pair,MyPair []Pair_)
	{
		for(int i =0;i<Pair_.length;i++)
		{
			if(Pair[i].key()!=Pair_[i].key())//one mismatch is enough for not being equal. we care for the row(i) value at each column(j) to be different.
			{    		
				return false;
			}
		}
		return true;
	}


	private void generateSonsIteration() {
		// TODO Auto-generated method stub
		queue.clear();	//in order to have a new comparison for current iteration
		Queen newQueenBoard=null;//queen board to be son of a k board
		if(isDone==false)
		{
			for(int i=0;i<K;i++)//K times for each best K chosen board
			{				

				//generate from that best K board more Boards(for each column)
				//for each row at same column , change the queen position
				for(int j=0;j<N;j++) 
				{
					for(int x=0;x<N;x++)
					{
						if(isDone==false)
						{
							if(Kboard.get(i).Board[x][j]!=1)
							{
								newQueenBoard = new Queen(N);
								newQueenBoard.copy(Kboard.get(i));//deep copy of father
								newQueenBoard.generateQueenBoardFromGivenBestBoard(j,x);
								newQueenBoard.CalcHioristic();
								if(newQueenBoard.score<bestEverScore)
									bestEverScore=newQueenBoard.score;
								if(newQueenBoard.score==0)//if Done
								{

									isDone=true;
									System.out.println("Solution"+" ( "+iterations+ " iterations " +") " +" and result is :");
									newQueenBoard.printBoard();
									break;
								}
								queue.add(newQueenBoard);
							}
						}



					}
				}

			}
		}
	}



	private ArrayList <Queen> pickBestKs() {
		// TODO Auto-generated method stub
		if(isDone==false)
		{
			iterations++;
			System.out.println("iteration "+ iterations);
			Kboard = new ArrayList<Queen>();//holds K Best Boards
			for(int i=0;i<K;i++)
			{
				Kboard.add(queue.peek());
				System.out.println( "Chosen Board Number "+(i+1));
				queue.peek().printBoard();
				queue.poll();
			}
		}
		return Kboard;
	}
	public static void main(String args[]) 
	{ 
		Scanner sc = new Scanner(System.in);
		int X, K, N;
		System.out.println("Please enter number of N (for creating NxN Board)");
		N = sc.nextInt();
		System.out.println("Please enter number of Iterations");
		X = sc.nextInt();
		System.out.println("Please enter number of K Boards");
		K = sc.nextInt();
		while(K>N*N)
		{
			System.out.println("K must be smaller than N*N, Please enter K element again");
			K= sc.nextInt();
		}
		BoardsManager B_Manager = new BoardsManager(X,K, N);
		B_Manager.Solve();
	} 
}
