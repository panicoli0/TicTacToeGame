using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public string gameMode;
    public GameObject cross, nought;

    [SerializeField] Text instructions;
    [SerializeField] Text xPlayerScoreText;
    [SerializeField] Text oPlayerScoreText;
    [SerializeField] Button restartButton;
    private int xPlayerScore;
    private int oPlayerScore;

    public enum Seed { EMPTY, CROSS, NOUGHT };

    public Seed Turn;

    // to keep track of the empty, cross and nought cells
    [SerializeField] GameObject[] allSpawns = new GameObject[9];

    // to maintain the state of the cell
    [SerializeField] Seed[] player = new Seed[9];

    private void Awake()
    {
        // to get the Game mode information from the previous scene
        GetPreviousScene();

        //Set first turn
        Turn = Seed.CROSS;

        // Set initial instruction
        instructions.text = "P1 Turn";

        // to maintain the state of the cell
        for (int i = 0; i < 9; i++)
            player[i] = Seed.EMPTY;

    }

    private void GetPreviousScene()
    {
        GameObject persistantObj = GameObject.FindGameObjectWithTag("PersistantObj") as GameObject;
        gameMode = persistantObj.GetComponent<PersistanceScript>().gameMode;
        Destroy(persistantObj);
    }

    public void Spawn(GameObject emptycell, int id)
    {
        // conditions to spawn cross or nought
        if (Turn == Seed.CROSS)
        {
            allSpawns[id] = Instantiate(cross, emptycell.transform.position, Quaternion.identity);
            player[id] = Turn;

            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                // change the instructions
                instructions.text = "P1 Win"; //todo: put all this in a method
                xPlayerScore = xPlayerScore + 1;
                xPlayerScoreText.gameObject.SetActive(true);
                xPlayerScoreText.text = "X Player Score: " + xPlayerScore;
                //reMatchButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);

            }
            else
            {
                // change the turn
                Turn = Seed.NOUGHT;

                // change the instructions
                instructions.text = "P2 Turn";
            }
        }

        else if (Turn == Seed.NOUGHT && gameMode == "pvp")
        {
            allSpawns[id] = Instantiate(nought, emptycell.transform.position, Quaternion.identity);
            player[id] = Turn;

            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                // change the instructions
                instructions.text = "P2 Win";//todo: put all this in a method
                oPlayerScore = oPlayerScore + 1;
                oPlayerScoreText.gameObject.SetActive(true);
                oPlayerScoreText.text = "O Player Score: " + oPlayerScore;
                //reMatchButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
            }
            else
            {
                // change the turn
                Turn = Seed.CROSS;

                // change the instructions
                instructions.text = "P1 Turn";
            }
        }

        if (Turn == Seed.NOUGHT && gameMode == "vscpu")
        {
            int bestScore = -1, bestPos = -1, score; //todo: to change difficulty

            for (int i = 0; i < 9; i++)
            {
                if (player[i] == Seed.EMPTY)
                {
                    player[i] = Seed.NOUGHT;
                    score = minimax(Seed.CROSS, player, -1000, +1000);
                    player[i] = Seed.EMPTY;

                    if (bestScore < score)
                    {
                        bestScore = score;
                        bestPos = i;
                    }
                }
            }

            if (bestPos > -1)
            {
                allSpawns[bestPos] = Instantiate(nought, allSpawns[bestPos].transform.position, Quaternion.identity);
                player[bestPos] = Turn;
            }


            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                // change the instructions
                instructions.text = "P2 Win"; //todo: put all this in a method
                oPlayerScore = oPlayerScore + 1;
                oPlayerScoreText.text = "O Player Score: " + oPlayerScore;
                //reMatchButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);

            }
            else
            {
                // change the turn
                Turn = Seed.CROSS;

                // change the instructions
                instructions.text = "P1 Turn";
            }
        }

        if (IsDraw())
        {
            // change the turn
            Turn = Seed.EMPTY;

            // change the instructions
            instructions.text = "It's a Draw";
            restartButton.gameObject.SetActive(true);
            //reMatchButton.gameObject.SetActive(true);


        }

        Destroy(emptycell);

    }

    bool IsAnyEmpty()
    {
        bool empty = false;
        for (int i = 0; i < 9; i++)
        {
            if (player[i] == Seed.EMPTY)
            {
                empty = true;
                break;
            }
        }
        return empty;
    }

    bool Won(Seed currPlayer)
    {
        bool hasWon = false;

        int[,] allConditions = new int[8, 3] { {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
                                                {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
                                                {0, 4, 8}, {2, 4, 6} };

        // check conditions
        for (int i = 0; i < 8; i++)
        {
            if (player[allConditions[i, 0]] == currPlayer &
                player[allConditions[i, 1]] == currPlayer &
                player[allConditions[i, 2]] == currPlayer)
            {
                hasWon = true;
                break;
            }
        }
        return hasWon;
    }

    bool IsDraw()
    {
        bool player1Won, player2Won, anyEmpty;

        // check if player-1 has won or not
        player1Won = Won(Seed.CROSS);

        // check if player-2 has won or not
        player2Won = Won(Seed.NOUGHT);

        // check if there is any empty cell or not
        anyEmpty = IsAnyEmpty();

        bool isDraw = false;

        if (player1Won == false & player2Won == false & anyEmpty == false)
            isDraw = true;

        return isDraw;
    }

    int minimax(Seed currPlayer, Seed[] board, int alpha, int beta)
    {

        if (IsDraw())
            return 0;

        if (Won(Seed.NOUGHT))
            return +1;

        if (Won(Seed.CROSS))
            return -1;


        int score;

        if (currPlayer == Seed.NOUGHT)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.NOUGHT;
                    score = minimax(Seed.CROSS, board, alpha, beta);
                    board[i] = Seed.EMPTY;

                    if (score > alpha)
                        alpha = score;

                    if (alpha > beta)
                        break;
                }
            }

            return alpha;
        }

        else
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.CROSS;
                    score = minimax(Seed.NOUGHT, board, alpha, beta);
                    board[i] = Seed.EMPTY;

                    if (score < beta)
                        beta = score;

                    if (alpha > beta)
                        break;
                }
            }

            return beta;
        }
    }
}