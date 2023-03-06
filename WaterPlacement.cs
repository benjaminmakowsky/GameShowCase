using UnityEngine;

namespace Farming
{
    public class WaterHole : Hole
    {
        public WaterHole(){
            hasAction = true;
            isCraftable = false;
            isPlacable = true;
            itemName = "Water Hole";
            isCollectable = false;
        }

        void Start(){
            GetComponent<SpriteRenderer>().enabled = false;
            checkSurroundingsAndUpdateOthers();
            setSprite();
            
        }

        private void OnDestroy(){
            gameObject.layer = 1;
            checkSurroundingsAndUpdateOthers();
        }


        //Checks surrounding tiles and updates those around it to match
        private void checkSurroundingsAndUpdateOthers()
        {
            
            Vector2 spritePosition = (Vector2)transform.position + middleCorrection;
            
        
            RaycastHit2D upCheck = Physics2D.Raycast(
                (Vector2)spritePosition + Vector2.up, 
                Vector2.up, 
                0.3f
                , LayerMask.GetMask("Farming"));
        
            RaycastHit2D downCheck = Physics2D.Raycast(
                (Vector2)spritePosition + Vector2.down, 
                Vector2.up, 
                0.3f
                , LayerMask.GetMask("Farming"));
        
            RaycastHit2D rightCheck = Physics2D.Raycast(
                (Vector2)spritePosition + Vector2.right, 
                Vector2.up, 
                0f
                , LayerMask.GetMask("Farming"));
        
            RaycastHit2D leftCheck = Physics2D.Raycast(
                (Vector2)spritePosition + Vector2.left, 
                Vector2.up, 
                0.3f
                , LayerMask.GetMask("Farming"));
            
            if (upCheck.collider != null) {
                Debug.Log(upCheck.collider.name);
            }
            if (upCheck.collider != null && upCheck.collider.name.StartsWith(itemName))
            {
                hasUp = true;
                updateHit(upCheck);
            }
        
            if (downCheck.collider != null && downCheck.collider.name.StartsWith(itemName))
            {
                hasDown = true;
                updateHit(downCheck);
            }

            if (leftCheck.collider != null && leftCheck.collider.name.StartsWith(itemName))
            {
                hasLeft = true;
                updateHit(leftCheck);
            }

            if (rightCheck.collider != null && rightCheck.collider.name.StartsWith(itemName))
            {
                hasRight = true;
                updateHit(rightCheck);
            }
        
        }
        
        //If the surrounding tile is the same type call update function
        private new void updateHit(RaycastHit2D hit)
        {
            string collidedName = hit.collider.name;
            Debug.Log($"Hit {collidedName}");
            if (collidedName.Contains(itemName))
            {
                Debug.Log($"Found {itemName}");
                hit.collider.GetComponent<WaterHole>().updateForNewHole();
            }
        }

        public void setMatchingSprite(Hole hole){
            hasUp = hole.hasUp;
            hasDown = hole.hasDown;
            hasLeft = hole.hasLeft;
            hasRight = hole.hasRight;
            setSprite();
        }
        
    }
}
