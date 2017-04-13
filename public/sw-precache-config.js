module.exports = {
  staticFileGlobs: [
    'index.html',
    'manifest.json',
    'bower_components/webcomponentsjs/webcomponents-lite.min.js',
  ],
  navigateFallback: 'index.html',
  runtimeCaching: [
    {
      urlPattern: /^https:\/\/4n5e3ppiq7.execute-api.us-east-2.amazonaws.com\/v1\/.*/,
      handler: 'cacheFirst',
      options: {
          cache: {
            maxEntries: 10,
            name: 'api-cache'
          }
      }
    },
    {
      urlPattern: /.*\/.*\.json/,
      handler: 'cacheFirst',
      options: {
          cache: {
            maxEntries: 10,
            name: 'json-cache'
          }
      }
    },
    {
      urlPattern: /\/articles\//,
      handler: 'fastest',
      options: {
          cache: {
            maxEntries: 10,
            name: 'articles-cache'
          }
      }
    }
  ],
  verbose: true,
  debug: true
};
