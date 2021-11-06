using System.Collections;
using UnityEngine;

namespace DeveloperConsole {

    public class ConsoleExamples : MonoBehaviour {

        [ConsoleCommand("test.method")]
        private void MyMethod() {
            Debug.Log("Called command 'test.method' successfully without parameters from Console!");
        }

        [ConsoleCommand("test.int", "3")]
        private void TestInt(int i) {
            Debug.Log(string.Format("Called command 'test.int' successfully with value: {0} from Console!", i));
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

        [ConsoleCommand("test.bool", "true")]            // Accepted boolean values: true, false, True, False, TRUE, FALSE
        private void TestBool(bool b) {
            Debug.Log(string.Format("Called command 'test.bool' successfully with value: {0} from Developer Console!", b));
        }

        [ConsoleCommand("test.string", "hello world")]
        private void TestString(string s) {
            Debug.Log(string.Format("Called command 'test.string' successfully with value: '{0}' from Developer Console!", s));
        }

        [ConsoleCommand("test.array", "hello, world")] // Allowed array separators: comma, dot, colon and semicolon
        private void StringArray(string[] stringArray) {
            for (int i = 0; i < stringArray.Length; i++) {
                Debug.Log(string.Format("Array index {0} with value: {1}", i, stringArray[i]));
            }
        }

        [ConsoleCommand("test.char")]
        private void TestChar(char c) {
            Debug.Log(string.Format("Called command 'test.char' successfully with value: {0} from Developer Console!", c));
        }

        [ConsoleCommand("test.vector2")]
        private void TestVector2(Vector2 v) {
            Debug.Log(string.Format("Called command 'test.vector2' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test.vector3")]
        private void TestVector3(Vector3 v) {
            Debug.Log(string.Format("Called command 'test.vector3' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test.vector4")]
        private void TestVector4(Vector3 v) {
            Debug.Log(string.Format("Called command 'test.vector3' successfully with value: {0} from Developer Console!", v));
        }

        [ConsoleCommand("test.quaternion")]
        private void TestQuaternion(Quaternion q) {
            Debug.Log(string.Format("Called command 'test.quaternion' successfully with value: {0} from Developer Console!", q));
        }

        [ConsoleCommand("test.byte")]
        private void TestByte(byte b) {
            Debug.Log(string.Format("Called command 'test.byte' successfully with value: {0} from Developer Console!", b));
        }

        [ConsoleCommand("test.coroutine")]
        private IEnumerator TestCoroutine() {
            Debug.Log("Coroutine start");
            yield return new WaitForSeconds(1.25f);
            Debug.Log("Coroutine end");
        }

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
            for (int i = 0; i < 151; i++) {
                Debug.Log(i);
                yield return delay;
            }
        }

        [ConsoleCommand("test.long")]
        private void PrintLongMessage() {
            var text = string.Empty;
            for (int i = 0; i < 151; i++) {
                text += i.ToString();
                Debug.Log(text);
            }
        }

        [ConsoleCommand("test.colors")]
        private void PrintColors() {
            Debug.Log("<color=red>R</color><color=green>G</color><color=blue>B</color>");
        }


        // If you have static MonoBehaviour methods,
        // make sure to handle errors (GameObject not existing for example)
        // or use ConsoleAPI.RemoveCommand().. when you no longer need the command.
        //[ConsoleCommand("test.method.static")]
        //private static void TestStatic() {
        //    Debug.Log("Called command 'test.method' successfully from Console!");
        //}

        // With all overloads
        // Default value (string), info (string), editorOnlyCommand (bool) and hiddenCommand (bool)
        [ConsoleCommand("test.hidden", null, null, true, true)]   
        private void HiddenCommand() {
            Debug.Log("Called hidden command 'hidden.command' successfully from Console!");
        }


        // Registering commands with ConsoleAPI.cs example.
        // this can be useful if you Instantiate objects runtime with MonoBehaviour script
        // as [ConsoleCommands] by default are only registered when scene is loaded,
        // so any [ConsoleCommands] added later will not be added.
        private void Start() {
            Console.RegisterCommand(this, "ManuallyRegisteredCommand", "test.manual", "", false);
            Console.RegisterCommand(this, "ManuallyRegisteredCommandInt", "test.manual.int", "42", false);
            Console.RegisterCommand(this, "ManualCoroutine", "test.manual.coroutine", "", false);
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

        [ConsoleCommand("test.remove.commads")]
        private void RemoveCommands() {
            Console.RemoveCommand("test.manual", true);
            Console.RemoveCommand("test.manual.int", true);
            Console.RemoveCommand("test.manual.coroutine", true);
        }
    }
}