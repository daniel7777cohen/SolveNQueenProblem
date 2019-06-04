//Daniel Cohen 203850029 Ben Efrat 305773806

package queen_lbs;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Random;


public class Queen {

	int[][] Board; 
	int N;
	public int score =0;
	MyPair[] indexes ;
	ArrayList<MyPair> changedIndexes ;
	public class MyPair
	{
		private final int i;
		private final int j;

		public MyPair(int aKey, int aValue)
		{
			i   = aKey;
			j = aValue;
		}

		public int key()   { return i; }
		public int value() { return j; }
	}

	public Queen(int N)//NxN matrix, K Boards, X iterations
	{
		changedIndexes = new ArrayList<MyPair>();
		indexes = new MyPair[N];
		this.N = N;
	}


	public void generateQueenBoard()
	{
		Random rand = new Random();
		Board = new int[N][N];
		for(int i=0;i<N;i++)
		{
			int n = rand.nextInt(N);
			Board[n][i] = 1;
			indexes[i] = new MyPair(n,i);
		}
	}

	public void generateQueenBoardFromGivenBestBoard( int j, int x)
	{
		Board[indexes[j].i][indexes[j].j]=0;//Son board replaces at his own board ,at specific column, 1 to be 0
		Board[x][j]= 1;//complete replacement of 1
		indexes[j] = new MyPair(x,j);//save that index for later
	}


	public void printBoard()
	{
		System.out.println("--------------------------");
		for(int i=0;i<N;i++)
		{
			for(int j=0;j<N;j++)
			{
				if(Board[i][j]==1)
					System.out.print("Q");
				else  System.out.print("X");
			}
			System.out.println();
		}
		System.out.println("Score = "+ score);
		System.out.println("--------------------------");

	}

	public void CalcHioristic() {
		for(int i=0;i<N;i++)
		{
			for(int j=0;j<N;j++)
			{
				if(Board[i][j]==1)
				{
					checkRow(i,j);
					checkColumn(i,j);
					checkDiagonalLine(i,j);

				}
			}
		}
	}

	private void checkDiagonalLine(int last_i,int last_j) {
		// TODO Auto-generated method stub
		int j,i;
		/* Check upper diagonal on left side */
		if(last_i!=0)
			for (i=last_i-1, j=last_j-1; i>=0 && j>=0; i--, j--) 
				if (Board[i][j] == 1) 
					score++; 

		if(last_i!=0)
			for (i=last_i-1, j=last_j+1; j<N && i>=0; i--, j++) 
				if (Board[i][j] == 1) 
					score++;
	}

	private void checkColumn(int last_i, int last_j) {
		// TODO Auto-generated method stub
		for(int i=last_i+1;i<N;i++)
		{
			if(Board[i][last_j]==1)
				score++;		
		}
	}

	private void checkRow(int last_i, int last_j) {

		// TODO Auto-generated method stub
		for(int j=last_j+1;j<N;j++)
		{
			if(Board[last_i][j]==1)
				score++;
		}
	}

	public void copy(Queen oldQueenBoard) {//Deep copy and initialization
		// TODO Auto-generated method stub
		this.Board = new int[N][N];
		for (int i = 0; i < oldQueenBoard.Board.length; i++) {
			this.Board[i] = Arrays.copyOf(oldQueenBoard.Board[i], oldQueenBoard.Board[i].length);

		}

		this.indexes = oldQueenBoard.indexes.clone();
		this.N = oldQueenBoard.N;
		this.changedIndexes = new ArrayList<MyPair>();

		this.score = 0;
	}



}
