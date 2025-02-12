using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class StatisticsLineRenderer : MonoBehaviour
{
    [SerializeField] private Color _lineColor = Color.blue;
    [SerializeField] private float _lineWidth = 2f;
    
    private void Awake()
    {
        var lineRenderer = GetComponent<LineRenderer>();
        
        // Базовые настройки
        lineRenderer.startColor = lineRenderer.endColor = _lineColor;
        lineRenderer.startWidth = lineRenderer.endWidth = _lineWidth;
        lineRenderer.useWorldSpace = false;
        
        // UI настройки
        lineRenderer.sortingOrder = 1;
        
        // Material
        if (lineRenderer.material == null || lineRenderer.material.shader.name != "Sprites/Default")
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }
}