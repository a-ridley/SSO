import { signLaunch } from "@/services/request";

const generateForm = destination => {
  let form = document.createElement("form");

  // Open launch requests in new tab
  form.target = "_blank";

  form.method = "POST";
  
  // Form should not be shown during launch
  form.style.display = "none";

  form.action = destination;

  return form;
};

const generateInput = (name, value) => {
  let input = document.createElement("input");

  input.name = name;
  input.value = value;

  return input;
};

const signAndLaunch = appId => {
  return signLaunch(appId)
    .then(launchData => {
      let form = generateForm(launchData.url);

      // Create an input within the form for each key
      for (let key in launchData.launchPayload) {
        let value = launchData.launchPayload[key];

        let input = generateInput(key, value);

        // Add the input to the form in order
        form.appendChild(input);
      }

      // Form must be bound to the dom due to browser security restrictions
      document.body.appendChild(form);

      form.submit();
    })
    .catch(err => {
      let code = (err.response || {}).status;

      switch (code) {
        case 401:
          throw new Error("Session is invalid. Please login again.");
        default:
          throw new Error(
            "An unexpected server error occurred. Please try again momentarily."
          );
      }
    });
};

export { signAndLaunch };
