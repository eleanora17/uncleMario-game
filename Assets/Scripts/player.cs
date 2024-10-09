// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using Nethereum.Web3;
// using Nethereum.Web3.Accounts;
// using Nethereum.Hex.HexTypes;
// using System.Threading.Tasks;
// using UnityEngine.UI;
// using Nethereum.Util;


// public class PlayerMovement : MonoBehaviour
// {
//     public TextMeshProUGUI countText;

//     [SerializeField] float jumpForce = 18f;
//     [SerializeField] float runSpeed = 500f;
//     // Start is called before the first frame update
//     float dirX;
//     private int countPine=0;
//     Rigidbody2D rb;
//     BoxCollider2D collider2D;
//     [SerializeField] LayerMask groundMask;
//     SpriteRenderer spriteRenderer;
//     Animator animator; 
//     bool gamePaused= false;
//     private enum MovementState {idle,run,jump,fall}

//     public string privateKey; // Add your private key
//     public string recipientAddress; // Add the recipient address
//     public TextMeshProUGUI balanceText;
//     private Web3 web3;
//     private string accountAddress;

//     void Start()
//     {
//         countText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
//         rb=GetComponent<Rigidbody2D>();
//         collider2D=GetComponent<BoxCollider2D>();
//         spriteRenderer=GetComponent<SpriteRenderer>();
//         animator = GetComponent<Animator>();

//         // Initialize Web3 instance
//         var account = new Account(privateKey);
//         web3 = new Web3(account, "http://127.0.0.1:7545"); // Ganache default URL

//         // Get the account address from the private key
//         accountAddress = account.Address;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         dirX= Input.GetAxisRaw("Horizontal");
//         if(Input.GetKeyDown(KeyCode.Space) && iSGrounded()){
//             rb.velocity=new Vector3(rb.velocity.x, jumpForce, 0f);
//         }
//         HandleAnimation();

//     }
//     private async void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.gameObject.CompareTag("Fruit"))
//         {
//             Destroy(other.gameObject);  // Destroy the coin
//             countPine+= 1;                    // Increment score
//             countText.text="PINE: "+ countPine;                
//             //UpdateScoreText();          // Update the score display
//         }

//         if(countPine==3)
//         {
//             try
//             {
//                 // Update the balance by sending 2 ETH
//                 var transactionHash = await SendEtherAsync(accountAddress, recipientAddress, 2);
//                 Debug.Log($"Transaction hash: {transactionHash}");

//                 // Update the balance display
//                 await UpdateBalanceDisplay();
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error: {ex.Message}");
//             }
//         }
//     }

//     async Task<string> SendEtherAsync(string fromAddress, string toAddress, decimal amountInEther)
//     {
//         // Convert Ether to Wei
//         var amountInWei = Web3.Convert.ToWei(amountInEther);

//         var transaction = new Nethereum.RPC.Eth.DTOs.TransactionInput
//         {
//             From = fromAddress,
//             To = toAddress,
//             Value = new HexBigInteger(amountInWei),
//             Gas = new HexBigInteger(21000), // Gas limit for standard transfers
//             GasPrice = new HexBigInteger(Web3.Convert.ToWei(20, UnitConversion.EthUnit.Gwei)) // Gas price in Gwei
//         };

//         // Send the transaction
//         var transactionHash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(transaction);
//         return transactionHash;
//     }

//     async Task UpdateBalanceDisplay()
//     {
//         // Get the balance of the account
//         var balance = await web3.Eth.GetBalance.SendRequestAsync(accountAddress);
//         var balanceInEther = Web3.Convert.FromWei(balance.Value);

//         // Update UI with balance
//         balanceText.text = $"Balance: {balanceInEther} ETH";
//         Debug.Log(balanceInEther);
//     }

//     void PauseGame(){
//         if(Input.GetKeyDown(KeyCode.Escape)){
//             gamePaused = !gamePaused;
//         }
//     }

//     private void FixedUpdate(){
//         rb.velocity = new Vector3(dirX* runSpeed * Time.deltaTime, rb.velocity.y, 0f);
//     }

//     bool iSGrounded(){
//         return Physics2D.BoxCast(collider2D.bounds.center,collider2D.bounds.size,0, Vector2.down, 0.1f, groundMask);
//     }


//     void HandleAnimation()
//     {
//         MovementState state;
//         if(dirX > 0)
//         {
//             spriteRenderer.flipX = false;
//             state = MovementState.run;
//         }
//         else if(dirX < 0)
//         {
//             spriteRenderer.flipX = true;
//             state = MovementState.run;
//         }
//         else
//         {
//             state = MovementState.idle;
//         }

//         if (rb.velocity.y > 0.1f)
//         {
//             state = MovementState.jump;
        
//         else if (rb.velocity.y < -0.1f)
//         {
//             state = MovementState.fall;
//         }
//         animator.SetInteger("state",(int)state);
//     }
// }