module.exports = {
  request: {
    https: {
      rejectUnauthorized: false,
    },
  },
  log: {
    level: 'trace',
    supportAnsiColors: true,
  },
  environments: {
    $shared: {
      host: 'https://mydoman',
    },
    dev: {
      baseUrl: 'https://localhost:52190',
    },
    prod: {
      user: 'mario',
      password: 'password$ecure123',
    },
  },
};
