using UnityEngine;

namespace Utility {
    
    /// <summary>
    /// This will paint an object to wrap arount the edge of your window 
    /// </summary>
    public class HomeDirectionHighlighter : MonoBehaviour {
        
        /// <summary>
        /// 2D camera ... only reads XY
        /// </summary>
        [SerializeField] private Transform _MainCamera;
        
        /// <summary>
        /// The object that is the home location (it can move !)
        /// </summary>
        [SerializeField] private Transform _HomeWaypoint;
        
        
        /// <summary>
        /// The object that will be moved; usually this object
        /// </summary>
        [SerializeField] private RectTransform _ObjectRectTransform;
        
        /// <summary>
        /// How large the object is on the when the home is north or south of the camera, vs east/west.
        /// </summary>
        [Header("Settings")]
        [SerializeField] private Vector2 _HorizontalSize = new Vector2(50,6); 
        [SerializeField] private Vector2 _VerticalSize = new Vector2(6,50);
        
        [SerializeField] private Renderer _HomeWaypointRenderer;

        private void Update() {

            if (_HomeWaypointRenderer == null)
                return;
            
            float EXTRA = _HomeWaypointRenderer.isVisible ? 100 : 0;
                

            Vector3 CAM = _MainCamera.position;
            Vector2 DIFF = _HomeWaypoint.position - CAM;
            
            float   CALCULATED_ANGLE = Mathf.Atan2( DIFF.y , DIFF.x ) * Mathf.Rad2Deg;

            ((RectTransform)transform).anchoredPosition = DetermineRectangleEdge(CALCULATED_ANGLE, Screen.width + EXTRA, Screen.height + EXTRA);

        }
        
        /// <summary>
        /// Source: https://stackoverflow.com/questions/4061576/finding-points-on-a-rectangle-at-a-given-angle 
        /// </summary>
        public Vector2 DetermineRectangleEdge(float aDegrees, float aWidth, float aHeight) {

            if (aDegrees < -90)
                aDegrees += 360f;

            float ANGLE = Mathf.Deg2Rad * aDegrees;
            float DIAG = Mathf.Atan2(aHeight, aWidth);
            float TANGENT = Mathf.Tan(ANGLE);

            Vector2 OUT = Vector2.zero;

            if (ANGLE > -DIAG && ANGLE <= DIAG)
            {
                OUT.x = aWidth / 2f;
                OUT.y = aWidth / 2f * TANGENT;
                _ObjectRectTransform.sizeDelta = _VerticalSize;
            }
            else if(ANGLE > DIAG && ANGLE <= Mathf.PI - DIAG)
            {
                OUT.x = aHeight / 2f / TANGENT;
                OUT.y = aHeight / 2f;
                _ObjectRectTransform.sizeDelta = _HorizontalSize;
            }
            else if(ANGLE > Mathf.PI - DIAG && ANGLE <= Mathf.PI + DIAG)
            {
                OUT.x = -aWidth / 2f;
                OUT.y = -aWidth / 2f * TANGENT;
                _ObjectRectTransform.sizeDelta = _VerticalSize;
            }
            else
            {
                OUT.x = -aHeight / 2f / TANGENT;
                OUT.y = -aHeight / 2f;
                _ObjectRectTransform.sizeDelta = _HorizontalSize;
            }

            return OUT;
            
        }

        public void AddRenderer(Renderer aHomeObjectRenderer) {
            _HomeWaypointRenderer = aHomeObjectRenderer;
        }
    }
    
}