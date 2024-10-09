using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using System.Threading.Tasks;
using TMPro;  // TextMeshPro for displaying balance

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float jumpForce = 18f;
    [SerializeField] float runSpeed = 500f;
    [SerializeField] TextMeshProUGUI balanceText;  // For displaying ETH balance
    [SerializeField] TextMeshProUGUI transactionText;  // For displaying transaction hash

    private float dirX;
    private Rigidbody2D rb;
    private BoxCollider2D collider2D;
    [SerializeField] LayerMask groundMask;
    private SpriteRenderer spriteRenderer;
    private Animator animator; 
    private bool gamePaused = false;
    private enum MovementState {idle, run, jump, fall}

    // Blockchain-related variables
    private Web3 web3;
    private string accountAddress;
    public string privateKey;  // Set this in Unity Inspector or hardcode
    public string recipientAddress;  // Set the recipient address

    private int countPine = 0;  // Pine counter

    void Start()
    {
        // Get Rigidbody and other components
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Initialize Web3 (Ganache default URL)
        var account = new Account(privateKey);
        web3 = new Web3(account, "http://127.0.0.1:7545"); // Ganache default URL

        // Store the address for transactions
        accountAddress = account.Address;

        // Update the balance display initially
        UpdateBalanceDisplay();
    }

    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0f);
        }
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(dirX * runSpeed * Time.deltaTime, rb.velocity.y, 0f);
    }

    void HandleAnimation()
    {
        MovementState state;
        if (dirX > 0)
        {
            spriteRenderer.flipX = false;
            state = MovementState.run;
        }
        else if (dirX < 0)
        {
            spriteRenderer.flipX = true;
            state = MovementState.run;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.fall;
        }
        animator.SetInteger("state", (int)state);
    }

    bool IsGrounded()
    {
        return Physics2D.BoxCast(collider2D.bounds.center, collider2D.bounds.size, 0, Vector2.down, 0.1f, groundMask);
    }

    private async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("fruit"))
        {
            Destroy(other.gameObject);  // Destroy the pine object
            countPine++;  // Increment pine count

            // Update the score display
            UpdateTransactionText($"PINES: {countPine}");

            // Check if the player has collected 3 pines
            if (countPine == 3)
            {
                try
                {
                    // Send 2 ETH to the recipient address
                    var transactionHash = await SendEtherAsync(accountAddress, recipientAddress, 2);
                    if (transactionHash != null)
                    {
                        UpdateTransactionText($"Transaction: {transactionHash}");
                    }

                    // Update the balance display
                    await UpdateBalanceDisplay();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error during transaction: {ex.Message}");
                }
            }
        }
    }

    // Function to send Ether
    public async Task<string> SendEtherAsync(string fromAddress, string toAddress, decimal amountInEther)
    {
        var amountInWei = Web3.Convert.ToWei(amountInEther);

        var transaction = new Nethereum.RPC.Eth.DTOs.TransactionInput
        {
            From = fromAddress,
            To = toAddress,
            Value = new HexBigInteger(amountInWei),
            Gas = new HexBigInteger(21000),  // Gas limit
            GasPrice = new HexBigInteger(Web3.Convert.ToWei(20, Nethereum.Util.UnitConversion.EthUnit.Gwei))  // Gas price
        };

        try
        {
            // Send transaction
            var transactionHash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(transaction);
            Debug.Log($"Transaction successful with hash: {transactionHash}");
            return transactionHash;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error sending transaction: {ex.Message}");
            return null;
        }
    }

    // Function to update balance display
    public async Task UpdateBalanceDisplay()
    {
        var balance = await web3.Eth.GetBalance.SendRequestAsync(accountAddress);
        var balanceInEther = Web3.Convert.FromWei(balance.Value);

        // Update UI
        balanceText.text = $"Balance: {balanceInEther} ETH";
        Debug.Log($"Balance: {balanceInEther} ETH");
    }

    // Function to update transaction text
    private void UpdateTransactionText(string message)
    {
        transactionText.text = message;
    }
}
