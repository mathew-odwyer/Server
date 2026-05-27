# 🚀 Release Checklist

---

## Pre-Release Verification

- [ ] All status checks have passed on `dev`
- [ ] Extensions are synced between server and client
- [ ] Run the server and client locally to confirm everything works

---

## Branch & Docs

- [ ] Create a new release branch: `release/vMajor.Minor.Patch`
- [ ] Update `docs/Versions/vMajor.Minor.Patch.md`
- [ ] Update `CHANGELOG.md` ensure _Unreleased_ section is current and all links resolve correctly

---

## Merge & Tag

- [ ] Push changes, review the PR, and merge
- [ ] Tag the release: `vMajor.Minor.Patch`

---

## GitHub Release

- [ ] Confirm the `release` workflow ran and the GitHub Release was created
- [ ] Update the GitHub Release description with the relevant `CHANGELOG.md` entries
- [ ] Attach the updated `Client-x64.zip` to the GitHub Release assets

---

## Deploy to Demo Environment

- [ ] SSH into the demo environment
  - [ ] Clone the repository
  - [ ] Tear down existing services: `docker-compose down -v`
  - [ ] Rebuild and start services: `docker-compose up --build -d`

---

## Sign-Off

- [ ] End-to-end test new features and bug fixes
- [ ] Chill out and watch everything burn
