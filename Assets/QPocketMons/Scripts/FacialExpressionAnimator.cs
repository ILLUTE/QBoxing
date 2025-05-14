using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniVRM10;

public class FacialExpressionAnimator : MonoBehaviour
{
    private Vrm10Instance vrmInstance;

    private ExpressionKey currentKey = ExpressionKey.Neutral;
    void Start()
    {
        // Get the Vrm10Instance component
        vrmInstance = GetComponent<Vrm10Instance>();
        if (vrmInstance == null)
        {
            Debug.LogError("Vrm10Instance not found on the VRM model.");
            return;
        }
    }

    public void SetExpression(ExpressionKey expressionKey, float weight)
    {
        // Get the Expression Manager
        var expressionManager = vrmInstance.Runtime.Expression;
        if (expressionManager == null)
        {
            Debug.LogError("Expression Manager not found.");
            return;
        }

        // Set expression weight
        expressionManager.SetWeight(currentKey, 0);
        expressionManager.SetWeight(expressionKey, weight);
        currentKey = expressionKey;
    }

    public void ResetExpressions()
    {
        var expressionManager = vrmInstance.Runtime.Expression;
        if (expressionManager != null)
        {
            expressionManager.SetWeight(currentKey, 0f);
        }
        
    }

    private IEnumerator ResetExpressionRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        ResetExpressions();
    }
}
