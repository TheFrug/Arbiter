The character creator happens on game start. It needs to save the data to some kind of file that gets accessed when the main scene loads.

During character creation, rather than allocate skill points, the player will be asked a few questions, and their answers will determine their base stats. From there, they can allocate a few remaining points for a point total of like 15. 

These questions should prime the player for the setting they are about to experience, as well as for the way they want to role-play Jean Rah.

Question example:
"What motivates you best?"
1. Helping others (Empathy)
2. Getting stronger (Force)
3. Learning (Insight)

### Flow
Each question will be its own panel that, when answered adds points to the stats in a struct called pendingStats or something. Once the questions are done, the player will see their pendingStats in a vertical line and arrows on either side. These arrows are how the player can increase their stats further with their remaining points, displayed in a box in the corner. Once they've allocated every point, the 'Submit' box lights up, allowing them to end character creation and move to the next scene. Doing this will save the pendingStats as characterStats, which I guess doesn't change? This part I am less sure about because I haven't made a character creator before.
### Questions
1. Should the character creator be its own scene? What managers and such need to be loaded into that scene and made DontDestroyOnLoad?
2. Should we use Yarn for the character creation choices? It might work fine, but it also might make more sense to use buttons and control the flow with our own script. 
3. What are some good questions to ask? I want to ride the line of being flavorful, as if these are questions the main character is asked in their job interview, as well as general enough that players can understand what is being asked without context of the wider game world.