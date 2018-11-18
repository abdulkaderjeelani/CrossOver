import { IdentityServerPage } from './app.po';

describe('identity-server App', function() {
  let page: IdentityServerPage;

  beforeEach(() => {
    page = new IdentityServerPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
