using System.Linq;
using UnityEngine;
using System;

namespace DeveloperConsole {

    class ConsoleUtils {

        public static string DeleteWhiteSpacesFromString(string text) {
            return text.ToCharArray()
             .Where(c => !Char.IsWhiteSpace(c))
             .Select(c => c.ToString())
             .Aggregate((a, b) => a + b);
        }

        public static string DeleteCharF(string text) {
            return text.Replace(ConsoleConstants.F, ConsoleConstants.EMPTY);
        }

        public static string DeleteWhiteSpace(string text) {
            if (text.StartsWith(ConsoleConstants.SPACE) || text.StartsWith(ConsoleConstants.T)) {
                return text.Remove(0, 1);
            }

            return text;
        }

        public static int CalcLevenshteinDistance(string a, string b) {
            // https://stackoverflow.com/a/9453762
            if (String.IsNullOrEmpty(a) && String.IsNullOrEmpty(b)) {
                return 0;
            }
            if (String.IsNullOrEmpty(a)) {
                return b.Length;
            }
            if (String.IsNullOrEmpty(b)) {
                return a.Length;
            }
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++) {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }

        public static bool IsRectTransformInsideSreen(RectTransform rectTransform, float screenEdgeOffset = 0) {
            bool isInside = false;
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            int visibleCorners = 0;
            Rect rect = new Rect(0, 0, Screen.width - screenEdgeOffset, Screen.height - screenEdgeOffset);
            foreach (Vector3 corner in corners) {
                if (rect.Contains(corner)) {
                    visibleCorners++;
                }
            }
            if (visibleCorners == 4) {
                isInside = true;
            }
            return isInside;
        }

        /// <summary>
        /// Show/hdie cursor
        /// </summary>
        public static void ShowCursor(bool show) {
            Cursor.visible = show;
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}