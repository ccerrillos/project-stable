module.exports = {
  staticFileGlobs: [
    'index.html',
    'manifest.json',
    'bower_components/webcomponentsjs/webcomponents-lite.min.js',
  ],
  navigateFallback: 'index.html',
  runtimeCaching: [
    {
      urlPattern: /^https:\/\/4n5e3ppiq7.execute-api.us-east-2.amazonaws.com\/v1\/.*cached.*/,
      handler: 'fastest',
      options: {
          cache: {
            maxEntries: 2,
            name: 'api-cache'
          }
      }
    },
    {
      urlPattern: /.*\/.*\.json/,
      handler: 'fastest',
      options: {
          cache: {
            maxEntries: 2,
            name: 'json-cache'
          }
      }
    }
  ],
  verbose: true,
  debug: true
};
