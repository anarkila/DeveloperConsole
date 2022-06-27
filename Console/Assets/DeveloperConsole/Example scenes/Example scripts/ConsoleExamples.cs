using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ConsoleExamples : MonoBehaviour {

        // this command doesn't take in any parameters
        // this can only be called:
        // 'test_method'
        [ConsoleCommand("test_method")]
        private void MyMethod() {
            Debug.Log("Called command 'test_method' successfully without parameters from Console!");
        }

        // this command takes in one int parameter
        // you must provide int to this command to work:
        // 'test_int {int}'
        [ConsoleCommand("test_int")]                                        // With just command
        //[ConsoleCommand("test_int", value:"7")]                           // With default value as string (default empty)
        //[ConsoleCommand("test_int", info:"calculates the...")]            // With command info (default empty)
        //[ConsoleCommand("test_int", debugOnlyCommand:true)]               // Debug only command, works only in Editor and Development Builds (default false)
        //[ConsoleCommand("test_int", hiddenCommandMinimalGUI:true)]        // Hidden command only in Minimal GUI but visible in Large GUI (default false)
        //[ConsoleCommand("test_int", hiddenCommand:true)]                  // Hidden command (default false)
        private void TestInt(int i) {
            Debug.Log(string.Format("Called command 'test_int' successfully with value: {0} from Console!", i));
        }


        // this command takes in optional int parameter
        // this command can be called two ways:
        // 'test_int_opt'
        // 'test_int_opt {int}'
        [ConsoleCommand("test_int_opt")]
        private void TestIntOpt(int i = 0) {
            Debug.Log(string.Format("Called command 'test_int_opt' successfully with value: {0} from Console!", i));
        }


        // this command takes two ints separated by comma
        // you must provide both ints to this command to work:
        // 'test_multi_int {int}, {int}'
        [ConsoleCommand("test_multi_int", value:"1, 2")]    
        private void TestMultiInt(int i, int j) {
            Debug.Log(string.Format("Called command 'test_multi_int' successfully with value: {0} and {1} from Console!", i, j));
        }

        // this command takes two optional ints separated by comma
        // this command can be called three ways:
        // 'test_multi.opt'
        // 'test_multi.opt {int}'
        // 'test_multi.opt {int}, {int}'
        [ConsoleCommand("test_multi_opt")] 
        private void TestIntIntOpt(int i = 0, int j = 1) {                                           
            Debug.Log(string.Format("Called command 'test_multi_opt' successfully with value: {0} and {1} from Console!", i, j));
        }

        // This command takes in one float parameter.
        [ConsoleCommand("test_float", "3.7")]
        private void TestFloat(float f) {
            Debug.Log(string.Format("Called command 'test_float' successfully with value: {0} from Console!", f));
        }

        [ConsoleCommand("test_double", "50.3")]
        private void TestDouble(double d) {
            Debug.Log(string.Format("Called command 'test_double' successfully with value: {0} from Console!", d));
        }

        [ConsoleCommand("test_decimal", "12.3")]
        private void TestDecimal(decimal de) {
            Debug.Log(string.Format("Called command 'test_decimal' successfully with value: {0} from Console!", de));
        }

        [ConsoleCommand("test_bool", "true")]           // Accepted boolean values: true, false, True, False, TRUE, FALSE
        private void TestBool(bool b) {
            Debug.Log(string.Format("Called command 'test_bool' successfully with value: {0} from Developer Console!", b));
        }

        [ConsoleCommand("test_string", "hello world")]
        private void TestString(string s) {
            Debug.Log(string.Format("Called command 'test_string' successfully with value: '{0}' from Developer Console!", s));
        }

        [ConsoleCommand("test_array", "hello, world")]  // Allowed separator: ',' (comma)
        private void StringArray(string[] stringArray) {
            for (int i = 0; i < stringArray.Length; i++) {
                Debug.Log(string.Format("Array index {0} with value: {1}", i, stringArray[i]));
            }
        }

        [ConsoleCommand("test_char")]
        private void TestChar(char c) {
            Debug.Log(string.Format("Called command 'test_char' successfully with value: {0} from Developer Console!", c));
        }

        // Unity Vector2/3/4 and Quaternions can be called like:
        // '{command_name} {x}, {y}'
        // '{command_name} {x}, {y}, {z}'
        // '{command_name} {x}, {y}, {z}, {w}'
        [ConsoleCommand("test_vector2", "1.0, 2.0")]
        private void TestVector2(Vector2 v) {
            Debug.Log(string.Format("Called command 'test_vector2' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test_vector3", "1.0, 2.0, 3.0")]
        private void TestVector3(Vector3 v) {
            Debug.Log(string.Format("Called command 'test_vector3' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test_vector4", "1.0, 2.0, 3.0, 4.0")]
        private void TestVector4(Vector4 v) {
            Debug.Log(string.Format("Called command 'test_vector3' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test_quaternion", "1.0, 2.0, 3.0, 4.0")]
        private void TestQuaternion(Quaternion q) {
            Debug.Log(string.Format("Called command 'test_quaternion' successfully with value: {0} from Developer Console!", q));
        }

        // Coroutines can be called just like other commands
        [ConsoleCommand("test_coroutine")]
        private IEnumerator TestCoroutine() {
            Debug.Log("Coroutine start");
            yield return new WaitForSeconds(1.25f);
            Debug.Log("Coroutine end");
        }

        // Coroutines are limited to one argument
        [ConsoleCommand("test_coroutine_float")]
        private IEnumerator TestCoroutineInt(float time) {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("Called Coroutine with parameter: " + time);
        }

        [ConsoleCommand("test_error")]
        private void PrintIntentialError() {
            Debug.Log("Showing intentional error.");
            throw new System.Exception();
        }

        [ConsoleCommand("test_numbers")]
        private IEnumerator PrintNumbers() {
            var delay = new WaitForSeconds(0.02f);
            for (int i = 0; i < 101; i++) {
                Console.Log(i);
                yield return delay;
            }
        }

        [ConsoleCommand("test_long")]
        private void PrintLongMessage() {
            var text = string.Empty;
            for (int i = 0; i < 151; i++) {
                text += i.ToString();
                Console.Log(text);
            }
        }


        // If you need to start coroutine with multiple parameters
        // make a normal method that starts the coroutine
        // this is because CommandDatabase.cs starts coroutine
        // by it's method name and that only takes in maximum of one parameter!
        /*[ConsoleCommand("test_coroutine_multi_int")]
        private void StartCoroutine(int i, int j) {
            StartCoroutine(CoroutineMultiInt(i, j));
        }

        private IEnumerator CoroutineMultiInt(int i, int j) {
            yield return new WaitForSeconds(0.02f);
            Debug.Log(string.Format("Called command 'test_coroutine_multi_int' successfully with value: {0} and {1} from Console!", i, j));
        }*/

#if UNITY_EDITOR
#if !UNITY_WEBGL
        [ConsoleCommand("test_threadedlog")]
        private void PrintLogFromAnotherThread() {
            Task.Run(() =>{
                // Note. Console.Log cannot be called from another thread.
                Debug.Log("Logged message from thread " + Thread.CurrentThread.ManagedThreadId);
            });
        }
# endif
#endif

        [ConsoleCommand("test_richtext")]
        private void PrintColors() {
            Debug.Log("<color=red>R</color><color=green>G</color><color=blue>B</color>");
        }

        [ConsoleCommand("test_colorlog")]
        private void PrintColoredLog() {
            Console.Log("This message is red!", Color.red);
        }

        // This command won't show up in predictions because it's set as hidden,
        // but it's still callable.
        [ConsoleCommand("test_hidden", null, null, false, true, true)]
        private void HiddenCommand() {
            Debug.Log("Called hidden command 'hidden.command' successfully from Console!");
        }

        // Registering commands with Console.cs example.
        // this is useful if you instantiate objects runtime with MonoBehaviour script that you want to control via console command,
        // as [ConsoleCommand] attributes are only registered when scene is loaded.
        // Objects that are instantiated runtime with [ConsoleCommand] attribute are not registered.
        private void OnEnable() {
            Console.RegisterCommand(this, "ManuallyRegisteredCommand", "test_manual");
            //Console.RegisterCommand(this, "ManuallyRegisteredCommand", "test_manual", "", "", false, false, false); // with all optional parameters

            Console.RegisterCommand(this, "ManuallyRegisteredCommandInt", "test_manual_int", "42");
            Console.RegisterCommand(this, "ManualCoroutine", "test_manual_coroutine");
        }

        private void ManuallyRegisteredCommand() {
            Debug.Log("Called manually registered command 'test_manual' successfully from Console!");
        }

        private void ManuallyRegisteredCommandInt(int i) {
            Debug.Log(string.Format("Called manually registered command 'test_manual_int' successfully with value: {0} from Console!", i));
        }

        private IEnumerator ManualCoroutine() {
            Debug.Log("Manual Coroutine start");
            yield return new WaitForSeconds(1.25f);
            Debug.Log("Manual Coroutine end");
        }

        // Example how to RemoveCommand with Console.cs
        private void OnDisable() {
            Console.RemoveCommand("test_manual");
            Console.RemoveCommand("test_manual_int");
            Console.RemoveCommand("test_manual_coroutine"/*, true*/); // optional boolean parameter to log whether removing command was successfull
        }


        // Examples how to change theme
        [ConsoleCommand("theme_dark", info:"Change GUI theme to Dark", debugOnlyCommand: true)]
        private void SetDarkTheme() {
            Console.SetGUITheme(ConsoleGUITheme.Dark);
        }

        [ConsoleCommand("theme_darker", info: "Change GUI theme to Darker", debugOnlyCommand: true)]
        private void SetDarkerTheme() {
            Console.SetGUITheme(ConsoleGUITheme.Darker);
        }

        [ConsoleCommand("theme_red", info: "Change GUI theme to Red", debugOnlyCommand:true)]
        private void SetRedTheme() {
            Console.SetGUITheme(ConsoleGUITheme.Red);
        }


        //// More Console.cs examples
        //private void Start() {

        //    // execute Console Command
        //    //Console.ExecuteCommand("test_int 42");

        //    //// Open Developer Console
        //    //Console.OpenConsole();
        //    //Console.ExecuteCommand("open");

        //    //// Close Developer Console
        //    //Console.CloseConsole();
        //    //Console.ExecuteCommand("close");

        //    //// Set GUI theme
        //    //Console.SetGUITheme(ConsoleGUITheme.Darker);

        //    //// Set Custom GUI Theme
        //    ConsoleColors consoleColors = new ConsoleColors {
        //        minimalGUIBackgroundColor = new Color(0f, 0f, 0f, 1f),
        //        minimalGUITextColor = new Color(1f, 0f, 0f, 1f),

        //        largeGUIBackgroundColor = new Color(0f, 0f, 0f, 0.97f),
        //        largeGUIBorderColor = new Color(0.2392157f, 0f, 0f, 1f),
        //        largeGUIHighlightColor = new Color(0.41f, 0.41f, 0.41f, 1f),
        //        largeGUIControlsColor = new Color(0f, 0f, 0f, 0.5f),
        //        largeGUIScrollbarBackgroundColor = new Color(0.2392157f, 0f, 0f, 1f),
        //        largeGUIScrollbarHandleColor = new Color(0f, 0f, 0f, 0.5f),
        //        largeGUITextColor = new Color(1f, 0f, 0f, 1f),
        //    };
        //    Console.SetCustomGUITheme(consoleColors);
        //}
    }
}