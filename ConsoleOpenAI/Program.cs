// See https://aka.ms/new-console-template for more information
using Azure.AI.OpenAI;

string nonAzureOpenAIApiKey = "your-api-key-from-platform.openai.com";

#region Stream chat messages with non-Azure OpenAI
var client = new OpenAIClient(nonAzureOpenAIApiKey, new OpenAIClientOptions());
int switch_on = 0; //Update this to valid example number.
switch (switch_on)
{
    case 1:
        #region Example 1
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = "gpt-3.5-turbo", // Use DeploymentName for "model" with non-Azure clients
            Messages =
            {
                new ChatRequestSystemMessage("You are a helpful assistant. You will talk like a pirate."),
                new ChatRequestUserMessage("Can you help me?"),
                new ChatRequestAssistantMessage("Arrrr! Of course, me hearty! What can I do for ye?"),
                new ChatRequestUserMessage("What's the best way to train a parrot?"),
            }
        };

        await foreach (StreamingChatCompletionsUpdate chatUpdate in client.GetChatCompletionsStreaming(chatCompletionsOptions))
        {
            if (chatUpdate.Role.HasValue)
            {
                Console.Write($"{chatUpdate.Role.Value.ToString().ToUpperInvariant()}: ");
            }
            if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
            {
                Console.Write(chatUpdate.ContentUpdate);
            }
        }
        //Output
        //ASSISTANT: Arrr, to train a parrot ye must be patient and use plenty o' positive reinforcement, like treats and praise. Teach them simple commands like "step up" or "come here" and be consistent in yer training. Remember, parrots be smart, so keep them entertained and stimulated with toys and puzzles! Aye, and don't forget to talk to them like a pirate, they be lovin' that! Arrr!

        #endregion
        break;
    case 2:
        #region Example 2
        //When explicitly requesting more than one Choice while streaming,
        //use the ChoiceIndex property on StreamingChatCompletionsUpdate to determine which Choice each update corresponds to.

        // A ChoiceCount > 1 will feature multiple, parallel, independent text generations arriving on the
        // same response. This may be useful when choosing between multiple candidates for a single request.
        var chatCompletionsOptions2 = new ChatCompletionsOptions()
        {
            DeploymentName = "gpt-3.5-turbo", // Use DeploymentName for "model" with non-Azure clients
            Messages = { new ChatRequestUserMessage("Write a limerick about bananas.") },
            ChoiceCount = 4
        };
        List<string> textBoxes = new();
        await foreach (StreamingChatCompletionsUpdate chatUpdate
            in client.GetChatCompletionsStreaming(chatCompletionsOptions2))
        {
            // Choice-specific information like Role and ContentUpdate will also provide a ChoiceIndex that allows
            // StreamingChatCompletionsUpdate data for independent choices to be appropriately separated.
            if (chatUpdate.ChoiceIndex.HasValue)
            {
                int choiceIndex = chatUpdate.ChoiceIndex.Value;
                if (chatUpdate.Role.HasValue)
                {
                    textBoxes.Add($"{chatUpdate.Role.Value.ToString().ToUpperInvariant()}: ");
                }
                if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
                {
                    textBoxes[choiceIndex] += chatUpdate.ContentUpdate;
                }
            }
        }
        foreach (var item in textBoxes)
        {
            Console.WriteLine(item);
        }
        //Output
        //ASSISTANT: There once was a fruit called a banana
        //It grew in a tropical savanna
        //With its yellow peel
        //And its sweet appeal
        //It was loved by both child and grandma
        //ASSISTANT: There once was a ripe yellow banana,
        //It slipped on a peel in Havana,
        //It fell with a squish,
        //But still got a wish,
        //To be in a smoothie nirvana.
        //ASSISTANT: There once was a fruit called a banana,
        //Yellow and sweet like a golden Havana.
        //It's great for a snack,
        //Packed with vitamins, that's a fact,
        //Eating one will make you say "hasta mañana."
        //ASSISTANT: There once was a fruit called banana
        //It was loved by both man and cabana
        //With a peel that was yellow
        //And a taste that was mellow
        //It's the best snack from here to savanna.
        #endregion
        break;
    default:
        Console.WriteLine("Invalid option " + switch_on);
        break;
}


#endregion

