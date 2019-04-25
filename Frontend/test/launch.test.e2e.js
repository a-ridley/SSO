// Deps
const puppeteer = require('puppeteer');
const { expect } = require('chai');

// Constants
const baseURL = process.env.BASE_URL || 'http://localhost:8080/#';
const registrationURL = `${baseURL}/register`;
const TYPE_DELAY = 0;

let animationTimeout = () => new Promise(resolve => setTimeout(resolve, 500))
let randomNum = (start, end) => Math.floor((Math.random() + start) * end)
let randomEmail = () => `${randomNum(1, 99999999)}@${randomNum(1, 99999999)}.com`

let fillReg = async page => {
    await page.type('#email', randomEmail(), {delay: TYPE_DELAY})
    await page.type('#password', 'G3jKk.jf0D](pl;cVa9j', {delay: TYPE_DELAY})
    await page.type('#confirm', 'G3jKk.jf0D](pl;cVa9j', {delay: TYPE_DELAY})

    await page.click('#dob')
    
    let yearSelector = 'ul.v-date-picker-years li:last-child'
    await page.waitForSelector(yearSelector)
    await animationTimeout()
    await page.click(yearSelector)
    
    let monthSelector = '.v-date-picker-table--month tr:last-child td:last-child button'
    await page.waitForSelector(monthSelector)
    await animationTimeout()
    await page.click(monthSelector)
    
    let daySelector = '.v-date-picker-table--date tr:last-child td:last-child button'
    await page.waitForSelector(daySelector)
    await animationTimeout()
    await page.click(daySelector)

    await page.type('#city', 'Los Angeles', {delay: TYPE_DELAY})
    await page.type('#state', 'California', {delay: TYPE_DELAY})
    await page.type('#country', 'United States', {delay: TYPE_DELAY})

    await page.click('#securityq1')
    await animationTimeout()
    await page.click('.menuable__content__active .v-select-list .v-list > div:first-child')
    await page.type('#securitya1', 'Answer 1.', {delay: TYPE_DELAY})

    await page.click('#securityq2')
    await animationTimeout()
    await page.click('.menuable__content__active .v-select-list .v-list > div:first-child')
    await page.type('#securitya2', 'Answer 2.', {delay: TYPE_DELAY})
    
    await page.click('#securityq3')
    await animationTimeout()
    await page.click('.menuable__content__active .v-select-list .v-list > div:first-child')
    await page.type('#securitya3', 'Answer 3.', {delay: TYPE_DELAY})
}

describe('launch', () => {
    let browser
    before(async () => {
        browser = await puppeteer.launch({ headless: false });
    })
    after(async () => {
        await browser.close();
    })

    describe('success', () => {
        let page
        before(async () => {
            page = await browser.newPage();
            await page.goto(registrationURL);
            await page.waitForSelector('main h1');
            await fillReg(page);
            await Promise.all([
                page.waitForNavigation(),
                page.click('main button')
            ])
        })
        after(async () => {
            await page.close();
        })

        it('can launch kfc app', async () => {
            await page.waitForSelector('#launchable');
            let pagesBefore = (await browser.pages()).length;
            page.click('#launchable');

            await browser.waitForTarget(target => target.url().indexOf('https://www.test.com') > -1);

            let pagesAfter = (await browser.pages()).length;

            expect(pagesBefore).to.equal(pagesAfter - 1);
        })
    })
})
