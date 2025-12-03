TODO: Refine this document and make it look more presentable.

- [ ] Ensure that all status checks have passed on dev
- [ ] Sync up all extensions between server and client just to be safe
- [ ] Create a new branch for the release (`release/vMajor.Minor.Patch`)
- [ ] Update `docs/Versions/vMajor.Minor.Patch.md`
- [ ] Update `CHANGELOG.md`, ensure _Unreleased_ is up-to-date and links work
- [ ] Tag the release version with vMajor.Minor.Patch
- [ ] Quickly run the server and client locally on your machine to make sure things are working
- [ ] Push the changes, review and merge the PR
- [ ] Once merged, the `release` workflow should run and create the GitHub Release
- [ ] Update the GitHub Release description to include the appropriate changes from `CHANGELOG.md`
- [ ] Update the GitHub Release assets to include the updated `Client-x64.zip`
- [ ] Update all other repositories (GMAnimations, GMUtilities, etc) if there were any changes
- [ ] SSH into demo environment
  - [ ] Clone the repository
  - [ ] Shutdown all services with `docker-compose down`
  - [ ] We're not using `docker-compose alpha`, so just `docker-compose up --build -d`
- [ ] Lastly, test end-to-end to make sure the new features, bug fixes, etc are all working
- [ ] Vibe Out 😎
