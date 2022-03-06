using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Elenesski.UI.Components {
    
    public class UIRadialMenu : MonoBehaviour {

        /// <summary>
        /// The root object that gets shown/hidden ... to handle 
        /// </summary>
        [SerializeField]
        private GameObject _HideRoot;

        /// <summary>
        /// How far do the buttons extend from the center of the menu
        /// </summary>
        [SerializeField]
        [Header("Default Radius if Radii is unspecified")]
        private float _Radius;

        /// <summary>
        /// Used to control the radius of a fixed number of items.  Uses _Radius if _Radii length is 0.
        /// </summary>
        [SerializeField]
        private List<float> _Radii;

        
        /// <summary>
        /// The starting angle of the radial buttons
        /// </summary>
        [SerializeField]
        [Header("Unfurl angles")]
        private float _StartAngle = 0;

        /// <summary>
        /// The ending angle of the buttons.
        /// </summary>
        [SerializeField]
        private float _EndAngle = 360;

        /// <summary>
        /// Identifies how fast the unfurl takes place in animation 
        /// </summary>
        [SerializeField]
        [Header("Animation Speed")]
        private float _AnimationLerp = 0.025f;

        /// <summary>
        /// Indicates whether the structure is closed or not.
        /// </summary>
        [SerializeField]
        private bool _RuntimeIsClosed = true;

        /// <summary>
        /// Returns TRUE when the menu is unfurled
        /// </summary>
        public bool IsUnfurled => ! _RuntimeIsClosed;

        [Header( "Actions" )]
        [SerializeField] private UnityEvent PreUnfurl;
        [SerializeField] private UnityEvent PostFurl;

        private void Awake() {
            _HideRoot.SetActive( false );
            _RuntimeIsClosed = true;
        }

        /// <summary>
        /// Lays out the elements
        /// </summary>
        public void Layout() {
            ShowUIElements( _Radius
                          , _StartAngle
                          , _EndAngle );
        }

        /// <summary>
        /// Toggles the raidal menu open/closed
        /// </summary>
        public void Toggle() {
            if ( _RuntimeIsClosed ) {
                Unfurl();
            } else {
                Furl();
            }
        }

        /// <summary>
        /// Does an animated unfurl
        /// </summary>
        public void Unfurl() {
            PreUnfurl.Invoke();
            
            ShowUIElements( 0 , _StartAngle , _StartAngle );
            _HideRoot.SetActive( true );
            StartCoroutine( UnfurlInternal() );
        }

        /// <summary>
        /// Coroutine to open the menu
        /// </summary>
        private IEnumerator UnfurlInternal() {
            
            float RADIUS = 0;
            float ANGLE  = _StartAngle;
            float T      = 0;

            if ( _Radii.Count() > 0 ) {
                _Radius = _Radii[GetComponentsInChildren<UIRadialMenuItem>( false ).Length - 1];
            }
            
            ShowUIElements( RADIUS , _StartAngle , ANGLE );
            yield return new WaitForEndOfFrame();

            while ( T < 1 ) {
                RADIUS = Mathf.Lerp( 0 , _Radius , T );
                ANGLE  = Mathf.Lerp( _StartAngle , _EndAngle , T );

                ShowUIElements( RADIUS , _StartAngle , ANGLE );
                T += _AnimationLerp;
                
                yield return new WaitForEndOfFrame();
            }
            
            ShowUIElements( _Radius , _StartAngle , _EndAngle );
            _RuntimeIsClosed = false;
            
        }
        
        /// <summary>
        /// Does an animated furl action
        /// </summary>
        public void Furl() {
            ShowUIElements( _Radius , _StartAngle , _EndAngle );
            StartCoroutine( FurlInternal() );
        }

        /// <summary>
        /// Coroutine to close the menu
        /// </summary>
        private IEnumerator FurlInternal() {
            float RADIUS = _Radius;
            float ANGLE  = _StartAngle;
            float T      = 1;

            ShowUIElements( RADIUS , ANGLE , _EndAngle );
            yield return new WaitForEndOfFrame();

            while ( T > 0 ) {
                RADIUS               = Mathf.Lerp( 0 , _Radius , T );
                ANGLE                = Mathf.Lerp( _StartAngle , _EndAngle , T );
                ShowUIElements( RADIUS , _StartAngle , ANGLE );
                T -= _AnimationLerp;

                yield return new WaitForEndOfFrame();
            }
            
            ShowUIElements( 0 , _StartAngle , _StartAngle );
            _RuntimeIsClosed = true;
            
            _HideRoot.SetActive( false );
            PostFurl.Invoke();

        }
        
        /// <summary>
        /// Shows all of the elements in the menu at a particular angle and radius. Used by furl and unfurl
        /// to show the menu items being seen/unseen.
        /// </summary>
        private void ShowUIElements( float aRadius , float aStartAngle , float aEndAngle ) {
        
            List<UIRadialMenuItem> OBJECTS = GetComponentsInChildren<UIRadialMenuItem>().ToList();
            float                  ANGLE   = aStartAngle;
            float                  DELTA   = ( aEndAngle - aStartAngle ) / (OBJECTS.Count() );

            foreach ( var ITEM in OBJECTS ) {
                Vector3 OFFSET = new Vector2( aRadius * Mathf.Sin( ANGLE * Mathf.Deg2Rad ) 
                                            , aRadius * Mathf.Cos( ANGLE * Mathf.Deg2Rad ) );

                ITEM.transform.localPosition =  OFFSET;
                ANGLE                        += DELTA;
            }
        
        }

        /// <summary>
        /// External method to unfurl the menu at a specific location
        /// </summary>
        /// <param name="aMousePosition">Position where the menu is revealed.</param>
        public void Unfurl( Vector3 aMousePosition ) {
            RectTransform RECT = transform as RectTransform;
            RECT.position = aMousePosition.Z( transform.position.z );
            Unfurl();
        }

        /// <summary>
        /// Hides the elements and closes the menu without animations.  Used when closing the dialog box.
        /// </summary>
        public void QuickFurl() {
            ShowUIElements( 0 , _StartAngle , _StartAngle );
            _RuntimeIsClosed = true;
            
            _HideRoot.SetActive( false );
        }

        /// <summary>
        /// Enables a certain number of options 
        /// </summary>
        /// <param name="aCount"></param>
        public void TurnOn( int aCount , Action<GameObject,int> aConfigureButton = null ) {
            List<UIRadialMenuItem> OBJECTS = GetComponentsInChildren<UIRadialMenuItem>(true).ToList();

            for ( int I = 0; I < OBJECTS.Count(); I++ ) {
                OBJECTS[I].gameObject.SetActive( I < aCount );
                if ( aConfigureButton != null )
                    aConfigureButton( OBJECTS[I].gameObject , I );
            }
        }
    }
 }
