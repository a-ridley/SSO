const puppeteer = require ('puppeteer');
// const sso = 'https://kfc-sso.com/#';
const sso = 'http://localhost:8080/#'
const appTitle = 'My Application';
const appEmail = 'app@email.com';
const appLaunchUrl = 'https://myapplication.com';
const appDeleteUrl = 'https://myapplication.com/delete';
const appHealthCheckUrl = 'https://myapplication.com/health';

function timeout(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
};

// Register with all valid fields
async function RegistrationValid() {

  let imgPath = 'Registration_Valid_';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/add');
  await page.screenshot({path: (imgPath + '01.png')});

  // Input all fields
  await page.type('#title', appTitle);
  await page.type('#launchUrl', appLaunchUrl);
  await page.type('#email', appEmail);
  await page.type('#deleteUrl', appDeleteUrl);
  await page.type('#healthCheckUrl', appHealthCheckUrl);
  await page.screenshot({path: (imgPath + '02.png')});

  // Click 'Register'
  await page.click('#btnRegister');
  await page.waitForSelector('#responseMessage');
  await page.screenshot({path: (imgPath + '03.png')});

  await browser.close();
  console.log('PASS: RegistrationValid');
};

// Register an existing application
async function RegistrationInvalidApp() {

  let imgPath = 'Registration_InvalidApp.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/add');

  // Input all fields
  await page.type('#title', appTitle);
  await page.type('#launchUrl', appLaunchUrl);
  await page.type('#email', appEmail);
  await page.type('#deleteUrl', appDeleteUrl);
  await page.type('#healthCheckUrl', appHealthCheckUrl);

  // Click 'Register'
  await page.click('#btnRegister');
  await timeout(1000);
  await page.screenshot({path: (imgPath)});

  await browser.close()
  console.log('PASS: RegistrationInvalidApp');
};

// Attempt to register with blank fields
async function RegistrationBlank() {

  let imgPath = 'Registration_BlankFields_';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/add');

  // Try to register with a blank title.
  await page.click('#btnRegister')
  await page.screenshot({path: (imgPath + '01.png')});

  // Try to register with a blank launch url
  await page.type('#title', appTitle);
  await page.click('#btnRegister');
  await page.screenshot({path: (imgPath + '02.png')});

  // Try to register with a blank email
  await page.type('#launchUrl', appLaunchUrl);
  await page.click('#btnRegister');
  await page.screenshot({path: (imgPath + '03.png')});

  // Try to register with a blank health check url
  await page.type('#email', appEmail);
  await page.click('#btnRegister');
  await page.screenshot({path: (imgPath + '04.png')});

  // Try to register with a blank user deletion url
  await page.type('#healthCheckUrl', appHealthCheckUrl);
  await page.click('#btnRegister');
  await page.screenshot({path: (imgPath + '05.png')});

  await browser.close();
  console.log('PASS: RegistrationBlank');
};

// Attempt to register with an invalid email
async function RegistrationInvalidEmail () {

  let imgPath = 'Registration_InvalidEmail.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/add');

  // Input title
  await page.type('#title', appTitle);

  // Input launch url
  await page.type('#launchUrl', appLaunchUrl);

  // Input invalid email
  await page.type('#email', 'email');    

  // Input health check url
  await page.type('#healthCheckUrl', appHealthCheckUrl);

  // Input user deletion url
  await page.type('#deleteUrl', appDeleteUrl);

  await page.click('#btnRegister');
  await timeout(1000);

  await page.screenshot({path: imgPath});

  await browser.close();
  console.log('PASS: RegistrationInvalidEmail');
};

// Attempt to register with an invalid launch url
async function RegistrationInvalidLaunchUrl () {

  let imgPath = 'Registration_InvalidLaunchUrl.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/add');

  // Input title
  await page.type('#title', appTitle);

  // Input invalid launch url
  await page.type('#launchUrl', 'app url');

  // Input email
  await page.type('#email', appEmail);    

  // Input health check url
  await page.type('#healthCheckUrl', appHealthCheckUrl);

  // Input user deletion url
  await page.type('#deleteUrl', appDeleteUrl);

  await page.click('#btnRegister');
  await timeout(1000);

  await page.screenshot({path: imgPath});

  await browser.close();
  console.log('PASS: RegistrationInvalidLaunchUrl');
};

// Attempt to register with an invalid user delete url
async function RegistrationInvalidDeleteUrl () {

  let imgPath = 'Registration_InvalidDeleteUrl.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/add');

  // Input title
  await page.type('#title', appTitle);

  // Input launch url
  await page.type('#launchUrl', appLaunchUrl);

  // Input email
  await page.type('#email', appEmail);

  // Input health check url
  await page.type('#healthCheckUrl', appHealthCheckUrl);

  // Input invalid user deletion url
  await page.type('#deleteUrl', 'app url');

  await page.click('#btnRegister');
  await timeout(1000);

  await page.screenshot({path: imgPath});

  await browser.close();
  console.log('PASS: RegistrationInvalidDeleteUrl');
};

// Attempt to register with an invalid user delete url
async function RegistrationInvalidHealthCheckUrl () {

  let imgPath = 'Registration_InvalidHealthCheckUrl.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/add');

  // Input title
  await page.type('#title', appTitle);

  // Input launch url
  await page.type('#launchUrl', appLaunchUrl);

  // Input email
  await page.type('#email', appEmail);

  // Input invalid health check url
  await page.type('#healthCheckUrl', 'health check');

  // Input user deletion url
  await page.type('#deleteUrl', appDeleteUrl);

  await page.click('#btnRegister');
  await timeout(1000);

  await page.screenshot({path: imgPath});

  await browser.close();
  console.log('PASS: RegistrationInvalidHealthCheckUrl');
};

// Generate a key with valid inputs
async function GenerateKeyValid() {

  let imgPath = 'GenerateKey_Valid_';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/key');
  await page.screenshot({path: (imgPath + '01.png')});

  // Input fields
  await page.type('#title', appTitle);
  await page.type('#email', appEmail);
  await page.screenshot({path: (imgPath + '02.png')});

  await page.click('#btnGenerate');
  await page.waitForSelector('#responseMessage');
  await page.screenshot({path: (imgPath + '03.png')});

  await browser.close();
  console.log('PASS: GenerateKeyValid');
};

// Generate a key with invalid inputs
async function GenerateKeyInvalidApp() {

  let imgPath = 'GenerateKey_InvalidApp.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/key');

  // Input fields
  await page.type('#title', 'my app');
  await page.type('#email', appEmail);

  await page.click('#btnGenerate');
  await timeout(1000);
  await page.screenshot({path: (imgPath)});

  await browser.close();
  console.log('PASS: GenerateKeyInvalidApp');
};

// Attempt to generate a key with blank fields
async function GenerateKeyBlank() {

  let imgPath = 'GenerateKey_BlankFields_';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/key');

  // Try to register with a blank title.
  await page.click('#btnGenerate')
  await page.screenshot({path: (imgPath + '01.png')});

  // Try to register with a blank email
  await page.type('#title', appTitle);
  await page.click('#btnGenerate');
  await page.screenshot({path: (imgPath + '02.png')});

  await browser.close();
  console.log('PASS: GenerateKeyBlank');
};

// Attempt to generate a key with an invalid email
async function GenerateKeyInvalidEmail() {

  let imgPath = 'GenerateKey_InvalidEmail.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/key');

  // Input title
  await page.type('#title', appTitle);
  await page.click('#btnGenerate')

  // Input invalid email
  await page.type('#email', 'email');
  await page.click('#btnGenerate');
  await timeout(1000);
  await page.screenshot({path: (imgPath)});

  await browser.close();
  console.log('PASS: GenerateKeyInvalidEmail');
};

// Delete an application
async function DeletionValid () {

  let imgPath = 'Deletion_Valid_';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/delete');
  await page.screenshot({path: (imgPath + '01.png')});

  // Input fields
  await page.type('#title', appTitle);
  await page.type('#email', appEmail);
  await page.screenshot({path: (imgPath + '02.png')});
  
  await page.click('#btnDelete');
  await page.waitForSelector('#deleteMessage');
  await page.screenshot({path: (imgPath + '03.png')});

  await browser.close();
  console.log('PASS: DeletionValid');
};

// Delete a non-existing application
async function DeletionInvalidApp () {

  let imgPath = 'Deletion_InvalidApp.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/delete');

  // Input fields
  await page.type('#title', 'my app');
  await page.type('#email', appEmail);
  
  await page.click('#btnDelete');
  await timeout(1000);
  await page.screenshot({path: (imgPath)});

  await browser.close();
  console.log('PASS: DeletionInvalidApp');
};

// Attempt to delete an application with blank fields
async function DeletionBlank() {

  let imgPath = 'Deletion_BlankFields_';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/delete');

  // Try to register with a blank title.
  await page.click('#btnDelete')
  await page.screenshot({path: (imgPath + '01.png')});

  // Try to register with a blank email
  await page.type('#title', appTitle);
  await page.click('#btnDelete');
  await page.screenshot({path: (imgPath + '02.png')});

  await browser.close();
  console.log('PASS: DeletionBlank');
};

// Attempt to delete an application with an invalid email
async function DeletionInvalidEmail() {

  let imgPath = 'Deletion_InvalidEmail.png';

  // Open browser and navigate to app registration page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/delete');

  // Input title
  await page.type('#title', appTitle);
  await page.click('#btnDelete')

  // Input invalid email
  await page.type('#email', 'email');
  await page.click('#btnDelete');
  await timeout(1000);
  await page.screenshot({path: (imgPath)});

  await browser.close();
  console.log('PASS: DeletionInvalidEmail');
};

async function Run(){
  await RegistrationValid()
  .then(function(){
    return RegistrationInvalidApp();
  })
  .then(function(){
    return GenerateKeyValid();
  })
  .then(function(){
    return DeletionValid();
  })
  .then(function(){
    return RegistrationBlank();
  })
  .then(function(){
    return RegistrationInvalidEmail();
  })
  .then(function(){
    return RegistrationInvalidLaunchUrl();
  })
  .then(function(){
    return RegistrationInvalidHealthCheckUrl();
  })
  .then(function(){
    return RegistrationInvalidDeleteUrl();
  })
  .then(function(){
    return GenerateKeyBlank();
  })
  .then(function(){
    return GenerateKeyInvalidEmail();
  })
  .then(function(){
    return GenerateKeyInvalidApp();
  })
  .then(function(){
    return DeletionBlank();
  })
  .then(function(){
    return DeletionInvalidEmail();
  })
  .then(function(){
    return DeletionInvalidApp();
  })
  .then(function(){
    console.log('DONE');
    return;
  })
  .catch(function(){
    console.log('FAIL');
  });
}

Run();