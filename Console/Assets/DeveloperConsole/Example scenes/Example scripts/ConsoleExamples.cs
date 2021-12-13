using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class ConsoleExamples : MonoBehaviour {

        // this command doesn't take in any parameters
        // this can only be called:
        // 'test.method'
        [ConsoleCommand("test.method")]
        private void MyMethod() {
            Debug.Log("Called command 'test.method' successfully without parameters from Console!");
        }

        // this command takes in one int parameter
        // you must provide int to this command to work:
        // 'test.int {int}'
        [ConsoleCommand("test.int")]                                        // With just command
        //[ConsoleCommand("test.int", value:"7")]                           // With default value as string (default empty)
        //[ConsoleCommand("test.int", info:"calculates the...")]            // With command info (default empty)
        //[ConsoleCommand("test.int", debugOnlyCommand:true)]               // Debug only command, works only in Editor and Development Builds (default false)
        //[ConsoleCommand("test.int", hiddenCommandMinimalGUI:true)]        // Hidden command only in Minimal GUI but visible in Large GUI (default false)
        //[ConsoleCommand("test.int", hiddenCommand:true)]                  // Hidden command (default false)
        private void TestInt(int i) {
            Debug.Log(string.Format("Called command 'test.int' successfully with value: {0} from Console!", i));
        }


        // this command takes in optional int parameter
        // this command can be called two ways:
        // 'test.int.opt'
        // 'test.int.opt {int}'
        [ConsoleCommand("test.int.opt")]
        private void TestIntOpt(int i = 0) {
            Debug.Log(string.Format("Called command 'test.int.opt' successfully with value: {0} from Console!", i));
        }


        // this command takes two ints separated by comma
        // you must provide both ints to this command to work:
        // 'test.multi.int {int}, {int}'
        [ConsoleCommand("test.multi.int", value:"1, 2")]    
        private void TestMultiInt(int i, int j) {
            Debug.Log(string.Format("Called command 'test.multi.int' successfully with value: {0} and {1} from Console!", i, j));
        }

        // this command takes two optional ints separated by comma
        // this command can be called three ways:
        // 'test.multi.opt'
        // 'test.multi.opt {int}'
        // 'test.multi.opt {int}, {int}'
        [ConsoleCommand("test.multi.opt")] 
        private void TestIntIntOpt(int i = 0, int j = 1) {                                           
            Debug.Log(string.Format("Called command 'test.multi.opt' successfully with value: {0} and {1} from Console!", i, j));
        }

        // this command takes in int, float and double where each is separated by comma
        // 'test.nums {int}, {float}, {double}
        [ConsoleCommand("test.nums")]
        private void TestNums(int i, float f, double d) {
            Debug.Log(string.Format("Called command 'test.nums' successfully with value: {0}, {1} and {2} from Console!", i, f, d));
        }

        [ConsoleCommand("test.float", "3.7")]
        private void TestFloat(float f) {
            Debug.Log(string.Format("Called command 'test.float' successfully with value: {0} from Console!", f));
        }

        [ConsoleCommand("test.double", "50.3")]
        private void TestDouble(double d) {
            Debug.Log(string.Format("Called command 'test.double' successfully with value: {0} from Console!", d));
        }

        [ConsoleCommand("test.decimal", "12.3")]
        private void TestDecimal(decimal de) {
            Debug.Log(string.Format("Called command 'test.decimal' successfully with value: {0} from Console!", de));
        }

        [ConsoleCommand("test.bool", "true")]           // Accepted boolean values: true, false, True, False, TRUE, FALSE
        private void TestBool(bool b) {
            Debug.Log(string.Format("Called command 'test.bool' successfully with value: {0} from Developer Console!", b));
        }

        [ConsoleCommand("test.string", "hello world")]
        private void TestString(string s) {
            Debug.Log(string.Format("Called command 'test.string' successfully with value: '{0}' from Developer Console!", s));
        }

        [ConsoleCommand("test.array", "hello, world")]  // Allowed separator: ',' (comma)
        private void StringArray(string[] stringArray) {
            for (int i = 0; i < stringArray.Length; i++) {
                Debug.Log(string.Format("Array index {0} with value: {1}", i, stringArray[i]));
            }
        }

        [ConsoleCommand("test.char")]
        private void TestChar(char c) {
            Debug.Log(string.Format("Called command 'test.char' successfully with value: {0} from Developer Console!", c));
        }

        [ConsoleCommand("test.vector2", "1.0, 2.0")]
        private void TestVector2(Vector2 v) {
            Debug.Log(string.Format("Called command 'test.vector2' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test.vector3", "1.0, 2.0, 3.0")]
        private void TestVector3(Vector3 v) {
            Debug.Log(string.Format("Called command 'test.vector3' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test.vector4", "1.0, 2.0, 3.0, 4.0")]
        private void TestVector4(Vector4 v) {
            Debug.Log(string.Format("Called command 'test.vector3' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test.quaternion", "1.0, 2.0, 3.0, 4.0")]
        private void TestQuaternion(Quaternion q) {
            Debug.Log(string.Format("Called command 'test.quaternion' successfully with value: {0} from Developer Console!", q));
        }

        [ConsoleCommand("test.coroutine")]
        private IEnumerator TestCoroutine() {
            Debug.Log("Coroutine start");
            yield return new WaitForSeconds(1.25f);
            Debug.Log("Coroutine end");
        }

        // Coroutines are limited to  one argument
        [ConsoleCommand("test.coroutine.int")]
        private IEnumerator TestCoroutineInt(int i) {
            Debug.Log("Testing Coroutine with parameter: " + i);
            yield return new WaitForSeconds(1.25f);
            Debug.Log("Coroutine end");
        }

        [ConsoleCommand("test.error")]
        private void PrintIntentialError() {
            Debug.Log("Showing intentional error:");
            throw new System.Exception();
        }

        [ConsoleCommand("test.numbers")]
        private IEnumerator PrintNumbers() {
            var delay = new WaitForSeconds(0.02f);
            for (int i = 0; i < 101; i++) {
                Console.Log(i);
                yield return delay;
            }
        }

        [ConsoleCommand("test.long")]
        private void PrintLongMessage() {
            var text = string.Empty;
            for (int i = 0; i < 151; i++) {
                text += i.ToString();
                Console.Log(text);
            }
        }

#if UNITY_EDITOR
#if !UNITY_WEBGL
        [ConsoleCommand("test.threadedlog")]
        private void PrintLogFromAnotherThread() {
            Task.Run(() =>{
                // Note. Console.Log cannot be called from another thread.
                Debug.Log("Logged message from thread " + Thread.CurrentThread.ManagedThreadId);
            });
        }
# endif
#endif

        [ConsoleCommand("test.richtext")]
        private void PrintColors() {
            Debug.Log("<color=red>R</color><color=green>G</color><color=blue>B</color>");
        }

        [ConsoleCommand("test.colorlog")]
        private void PrintColoredLog() {
            Console.Log("This message is red!", Color.red);
        }

        // This command won't show up in predictions because it's set as hidden,
        // but it's still callable.
        [ConsoleCommand("test.hidden", null, null, false, true, true)]
        private void HiddenCommand() {
            Debug.Log("Called hidden command 'hidden.command' successfully from Console!");
        }

        // Registering commands with Console.cs example.
        // this is useful if you instantiate objects runtime with MonoBehaviour script that you want to control via console command,
        // as [ConsoleCommand] attributes are only registered when scene is loaded.
        // Objects that are instantiated runtime with [ConsoleCommand] attribute are not registered.
        private void OnEnable() {
            Console.RegisterCommand(this, "ManuallyRegisteredCommand", "test.manual", "", "", false, false, false);
            Console.RegisterCommand(this, "ManuallyRegisteredCommandInt", "test.manual.int", "42", "", false, false, false);
            Console.RegisterCommand(this, "ManualCoroutine", "test.manual.coroutine", "", "", false, false, false);
        }

        private void ManuallyRegisteredCommand() {
            Debug.Log("Called manually registered command 'test.manual' successfully from Console!");
        }

        private void ManuallyRegisteredCommandInt(int i) {
            Debug.Log(string.Format("Called manually registered command 'test.manual.int' successfully with value: {0} from Console!", i));
        }

        private IEnumerator ManualCoroutine() {
            Debug.Log("Manual Coroutine start");
            yield return new WaitForSeconds(1.25f);
            Debug.Log("Manual Coroutine end");
        }

        // Example how to RemoveCommand with Console.cs
        private void OnDisable() {
            Console.RemoveCommand("test.manual");
            Console.RemoveCommand("test.manual.int");
            Console.RemoveCommand("test.manual.coroutine", true); // optional boolean parameter to log whether removing command was successfull
        }
    }
}